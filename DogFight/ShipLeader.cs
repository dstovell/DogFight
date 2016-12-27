using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FluffyUnderware.Curvy.Controllers;
using FluffyUnderware.Curvy;

namespace DogFight
{

public class ShipLeader : MonoBehaviour
{
	public SWS.PathManager currentPath;

	public SWS.splineMove mover;
	public SWS.PathManager pathManager;

	public SplineController controller;

	public SplineGroup splineGroup;
	public FluffyUnderware.Curvy.CurvySpline spline;

	private float speed = 0f;

	private bool useController = true;

	// Use this for initialization
	void Start ()
	{
		this.mover = this.gameObject.GetComponent<SWS.splineMove>();
		this.pathManager = this.gameObject.GetComponent<SWS.PathManager>();
		this.controller = this.gameObject.GetComponent<SplineController>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		//this.mover.animEaseType
	}

	private SWS.PathManager GeneratePathManager(string name, List<Vector3> points)
	{
		GameObject obj = new GameObject();
		obj.name = name + "Path";

		SWS.PathManager path = obj.AddComponent<SWS.PathManager>();
		path.waypoints = new Transform[points.Count];

		for (int i=0; i<points.Count; i++)
		{
			GameObject nodeObj = new GameObject("node" + i);
			nodeObj.transform.position = points[i];

			nodeObj.transform.SetParent(obj.transform);
			path.waypoints[i] = nodeObj.transform;
		}

		return path;
	}

	public void Pause(float secs)
	{
		if (this.mover != null)
		{
			this.mover.Pause(secs);
		}

		if (this.controller != null)
		{		
			this.controller.Pause();
		}
	}

	public void SetSpeed(float speed_)
	{
		this.speed = speed_;

		if (this.mover != null)
		{
			this.mover.speed = this.speed;
		}

		if (this.controller != null)
		{		
			this.controller.Speed = this.speed;
		}
	}


	public void SetSplineGroup(SplineGroup group)
	{
		this.splineGroup = group;
		this.splineGroup.SetGroup(this);
	}

	public void MoveSpline(FluffyUnderware.Curvy.CurvySpline spline_)
	{
		if (this.speed == 0)
		{
			return;
		}

		this.spline = spline_;

		Vector3 currentPos = this.transform.position;
		Vector3 nearestPoint;
		float nearestT = this.spline.GetNearestPointTF(currentPos, out nearestPoint);

		float duration = Vector3.Distance(currentPos, nearestPoint) / this.speed;
		this.controller.SwitchTo(this.spline, nearestT, duration);
	}

	public void MovePath(List<Vector3> points, bool loop = false)
	{
		if (this.currentPath != null)
		{
			GameObject.DestroyImmediate(this.currentPath.gameObject);
			this.currentPath = null;
		}

		this.currentPath = this.GeneratePathManager(this.gameObject.name, points);
		this.mover.moveToPath = true;
		this.mover.loopType = loop ? SWS.splineMove.LoopType.pingPong : SWS.splineMove.LoopType.none;
		this.mover.SetPath(this.currentPath);
	}

	public void ChangeSpline(Vector3 direction, float maxAngle)
	{
		if (this.splineGroup == null)
		{
			return;
		}

		float currentT = this.controller.Position;
		FluffyUnderware.Curvy.CurvySpline newSpline = this.splineGroup.GetChangeSpline(this.spline, currentT, direction, maxAngle);
		if (newSpline == null)
		{
			return;
		}

		this.MoveSpline(newSpline);
	}

	public List<Vector3> GetChangeSplineDirections()
	{
		if ((this.splineGroup == null) || (this.spline == null))
		{
			return new List<Vector3>();
		}

		return this.splineGroup.GetChangeSplineDirections(this.spline, this.controller.Position);
	}

	public bool IsAtDestination()
	{
		if ((this.currentPath == null) || (this.mover == null) || (this.mover.waypoints == null))
		{
			return false;
		}

		bool isLastPoint = (this.mover.currentPoint == (this.mover.waypoints.Length-1));
		if (!isLastPoint)
		{
			return false;
		}

		float distanceToPoint = Vector3.Distance(this.transform.position, this.mover.waypoints[this.mover.currentPoint]);
		return (distanceToPoint == 0f);
	}

	public Vector3 GetNextWayPoint(float minDistance)
	{
		Vector3 nextPos = Vector3.zero;
		if ((this.currentPath == null) || (this.mover == null) || (this.mover.waypoints == null))
		{
			return nextPos;
		}

		Vector3 currentPos = this.transform.position;
		int start = this.mover.currentPoint;
		for (int i=start; i<this.mover.waypoints.Length; i++)
		{
			float dist = Vector3.Distance(currentPos, this.mover.waypoints[i]);
			if (dist >= minDistance)
			{
				//Debug.LogError("GetNextWayPoint start=" + start + " i=" + i + " dist=" + dist + " minDistance=" + minDistance );
				nextPos = this.mover.waypoints[i];
				break;
			}
		}

		return nextPos;
	}

	public bool IsOnCurrentPath(Vector3 pos)
	{
		if ((this.currentPath == null) || (this.mover == null) || (this.mover.waypoints == null))
		{
			return false;
		}

		for (int i=0; i<this.mover.waypoints.Length; i++)
		{
			if (this.mover.waypoints[i] == pos)
			{
				return true;
			}
		}

		return false;
	}

	public bool IsMoving()
	{
		return (this.currentPath != null);
	}

	public void DestroyPath()
	{
		if (this.currentPath != null)
		{
			GameObject.DestroyImmediate(this.currentPath.gameObject);
			this.currentPath = null;
		}
	}

	public void Stop()
	{
		if (this.mover != null)
		{
			this.mover.Stop();
		}
		DestroyPath();
	}
}

}
