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
	public float DesiredLeaderDist = 5f;

	public ShipLeader Leader;

	private Rigidbody rb;

	private float currentSpeed = 0f;

	// Use this for initialization
	void Start() 
	{
		this.InitMessenger("ShipController");
		this.rb = this.gameObject.GetComponent<Rigidbody>();

		this.Leader.gameObject.transform.SetParent(null);
	}

	void Update() 
	{
		Vector3 desiredDirection = (this.Leader.transform.position - this.transform.position).normalized;
		Quaternion desiredRotation = Quaternion.LookRotation(desiredDirection);
		this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, desiredRotation, this.RotateSpeed*Time.deltaTime);

		float distanceToLeader = Vector3.Distance(this.transform.position, this.Leader.transform.position);
		this.currentSpeed = this.MoveSpeed * Mathf.Min(distanceToLeader/this.DesiredLeaderDist, 2.0f);
		this.transform.position += this.transform.forward * this.currentSpeed * Time.deltaTime;

		UpdateThrusters();
	}

	private void UpdateThrusters()
	{
	}

	public bool IsFacing(Vector3 dir)
	{
		return (Vector3.Angle(this.transform.forward, dir) < 0.1);
	}

	public float GetSpeed()
	{
		return currentSpeed;
	}

	public bool IsMoving()
	{
		return this.Leader.IsMoving();
	}

	public void MovePath(List<Transform> points)
	{
		this.Leader.MovePath(points);
	}

	public void Stop()
	{
		this.Leader.Stop();
	}

	void OnCollisionEnter(Collision info)
	{
		//GameObject obj = info.collider.gameObject;
	}
}

}
