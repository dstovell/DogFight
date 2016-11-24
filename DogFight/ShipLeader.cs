using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

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
	
	}

	private SWS.PathManager GeneratePathManager(string name, List<Transform> points)
	{
		GameObject obj = new GameObject();
		obj.name = name + "Path";

		SWS.PathManager path = obj.AddComponent<SWS.PathManager>();
		path.waypoints = new Transform[points.Count];

		for (int i=0; i<points.Count; i++)
		{
			points[i].SetParent(obj.transform);
			path.waypoints[i] = points[i];
		}

		return path;
	}

	public void MovePath(List<Transform> points)
	{
		this.currentPath = this.GeneratePathManager("Ship", points);
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
