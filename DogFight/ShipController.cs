using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using DSTools;

namespace DogFight
{

public class ShipController : MessengerListener
{
	public enum RotateMode
	{
		Idle,
		Turn,
		SpiralTurn
	}
	public RotateMode rotateMode = RotateMode.Idle;

	public float MoveSpeed = 1f;
	public float RotateSpeed = 1f;
	public float DesiredLeaderDist = 5f;

	public float IdleRotateSpeed = 1f;
	public float MaxIdleRotate = 20f;

	public ShipLeader Leader;
	public Seeker Seeker;
	public GameObject Rotator;

	private Rigidbody rb;

	private float currentSpeed = 0f;

	public Quaternion StartRotate = Quaternion.identity;
	public Quaternion GoalRotate = Quaternion.identity;

	public bool RotateStarted = false;
	private float NextIdleRotateDir = 1;

	// Use this for initialization
	void Start() 
	{
		this.InitMessenger("ShipController");
		this.rb = this.gameObject.GetComponent<Rigidbody>();

		this.Leader.gameObject.transform.SetParent(null);
		this.Leader.SetSpeed(this.MoveSpeed);

		if (this.Rotator != null)
		{
			this.StartRotate = this.Rotator.transform.rotation;
		}

		this.MoveTo( Arena.Instance.EndB.transform.position );
	}

	void Update() 
	{
		if (!this.IsMoving())
		{
			this.MoveTo( Arena.Instance.EndA.transform.position );
		}

		Vector3 desiredDirection = (this.Leader.transform.position - this.transform.position).normalized;
		Quaternion desiredRotation = Quaternion.LookRotation(desiredDirection);
		this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, desiredRotation, this.RotateSpeed*Time.deltaTime);
		float turnAngleRemaining = Quaternion.Angle(this.transform.rotation, desiredRotation);
		if (turnAngleRemaining < 0.1f)
		{
			this.SetRotateMode(RotateMode.Idle);
		}
		else
		{
			this.SetRotateMode(RotateMode.Turn);
		}

		float distanceToLeader = Vector3.Distance(this.transform.position, this.Leader.transform.position);
		this.currentSpeed = this.MoveSpeed * Mathf.Min(distanceToLeader/this.DesiredLeaderDist, 2.0f);
		this.transform.position += this.transform.forward * this.currentSpeed * Time.deltaTime;

		UpdateThrusters();
		UpdateRotator();
	}

	private void UpdateThrusters()
	{
	}

	private void SetRotateMode(RotateMode m)
	{
		if (this.rotateMode == m)
		{
			return;
		}

		this.rotateMode = m;
		this.RotateStarted = true;

		if (this.rotateMode == RotateMode.Idle)
		{
			this.GoalRotate = this.StartRotate;
		}
		else if (this.rotateMode == RotateMode.Turn)
		{
			this.GoalRotate = this.StartRotate;
		}
		else if (this.rotateMode == RotateMode.SpiralTurn)
		{
			this.GoalRotate = this.StartRotate;
		}
	}

	private void UpdateRotator()
	{
		if (this.Rotator == null)
		{
			return;
		}

		if (this.rotateMode == RotateMode.Idle)
		{
			if (this.RotateStarted)
			{
				this.Rotator.transform.rotation = Quaternion.RotateTowards(this.Rotator.transform.rotation, this.GoalRotate, this.IdleRotateSpeed*Time.deltaTime);
				if (this.Rotator.transform.rotation == this.GoalRotate) 
				{
					this.GoalRotate = Quaternion.identity;
					this.RotateStarted = false;
				}
			}
			else 
			{
				float minRotate = 0.2f;
				this.Rotator.transform.rotation = Quaternion.RotateTowards(this.Rotator.transform.rotation, this.StartRotate, this.IdleRotateSpeed*Time.deltaTime);
				if (this.Rotator.transform.rotation == this.StartRotate)
				{
					float randomT = (this.NextIdleRotateDir > 0) ? Random.Range(minRotate, 1f) : Random.Range(-1f, -1*minRotate);
					float randomAngle = this.MaxIdleRotate*randomT;
					this.GoalRotate = Quaternion.AngleAxis(randomAngle, this.Rotator.transform.forward) * this.StartRotate;
					this.RotateStarted = true;

					this.NextIdleRotateDir *= -1f;
				}
			}
		}
		else if (this.rotateMode == RotateMode.Turn)
		{
			if (this.RotateStarted)
			{
				this.Rotator.transform.rotation = Quaternion.RotateTowards(this.Rotator.transform.rotation, this.GoalRotate, this.RotateSpeed*Time.deltaTime);
				if (this.Rotator.transform.rotation == this.GoalRotate) 
				{
					this.GoalRotate = Quaternion.identity;
					this.RotateStarted = false;
				}
			}
		}
		else if (this.rotateMode == RotateMode.SpiralTurn)
		{
		}
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

	public void MoveTo(Vector3 to)
	{
		this.Seeker.StartPath(this.Leader.transform.position, to, OnMovePathComplete);
	}

	public void MovePath(Vector3 from, Vector3 to)
	{
		this.Seeker.StartPath(from, to, OnMovePathComplete);
	}

	private void OnMovePathComplete(Path p) 
	{
		p.Claim(this);
		this.MovePath(p.vectorPath);
	}

	public void MovePath(List<Vector3> points)
	{
		this.Leader.MovePath(points, true);
	}

	public void Stop()
	{
		this.Leader.Stop();
	}

	public void FindPath(Vector3 from, Vector3 to)
	{
		this.Seeker.StartPath(from, to, OnPathComplete);
	}

	private void OnPathComplete (Path p) 
	{
		p.Claim(this);
	}

	void OnCollisionEnter(Collision info)
	{
		//GameObject obj = info.collider.gameObject;
	}
}

}
