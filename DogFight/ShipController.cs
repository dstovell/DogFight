using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DSTools;

namespace DogFight
{

public class ShipController : MessengerListener
{
	public float MoveSpeed = 1f;
	public float RotateSpeed = 1f;

	private Rigidbody rb;

	public SWS.PathManager currentPath;

	public Quaternion desiredRotation;

	private SWS.splineMove mover;
	private SWS.PathManager pathManager;

	private CapsuleCollider capsule;

	// Use this for initialization
	void Start() 
	{
		this.InitMessenger("ShipController");
		this.rb = this.gameObject.GetComponent<Rigidbody>();
		this.mover = this.gameObject.GetComponent<SWS.splineMove>();
		this.pathManager = this.gameObject.GetComponent<SWS.PathManager>();

		//this.mover.lockRotation = DG.Tweening.AxisConstraint.X;
	}

	void Update () 
	{
		
	}

	public bool IsFacing(Vector3 dir)
	{
		return (Vector3.Angle(this.transform.forward, dir) < 0.1);
	}

	public bool IsMoving()
	{
		return (this.currentPath != null);
	}

	public SWS.PathManager GeneratePathManager(string name, List<Transform> points)
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
		this.currentPath = this.GeneratePathManager("PlayerRow", points);
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

	void OnCollisionEnter(Collision info)
	{
		//GameObject obj = info.collider.gameObject;
	}
}

}
