using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DogFight
{

public class ShipLeader : MonoBehaviour
{

	public SWS.PathManager currentPath;

	public SWS.splineMove mover;
	public SWS.PathManager pathManager;

	// Use this for initialization
	void Start ()
	{
		this.mover = this.gameObject.GetComponent<SWS.splineMove>();
		this.pathManager = this.gameObject.GetComponent<SWS.PathManager>();
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

	public void SetSpeed(float speed)
	{
		this.mover.speed = speed;
	}

	public void MovePath(List<Vector3> points, bool loop = false)
	{
		this.currentPath = this.GeneratePathManager("Ship", points);
		this.mover.moveToPath = true;
		this.mover.loopType = loop ? SWS.splineMove.LoopType.pingPong : SWS.splineMove.LoopType.none;
		this.mover.SetPath(this.currentPath);
	}

	public bool IsMoving()
	{
		return (this.currentPath != null);
	}

	public void Stop()
	{
		if (this.mover != null)
		{
			this.mover.Stop();
		}
		if (this.currentPath != null)
		{
			GameObject.Destroy(this.currentPath.gameObject);
			this.currentPath = null;
		}
	}
}

}
