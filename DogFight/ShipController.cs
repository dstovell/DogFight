using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using DSTools;

namespace DogFight
{

public class ShipController : MessengerListener
{
	static private List<ShipController> Ships = new List<ShipController>();

	public enum PilotType
	{
		AI,
		Human
	}
	public PilotType Pilot = PilotType.AI;

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

	public ArenaSpawn Spawn;

	public SpriteObjectTracker Reticle;

	public ShipWeapon PrimaryWeapon;

	public List<ShipController> HostileShips = new List<ShipController>();

	private Rigidbody rb;

	private float currentSpeed = 0f;

	public float StartRotate = 0f;
	public float GoalRotate = 0f;
	public float CurrentRotate = 0f;

	public bool RotateStarted = false;
	private float NextIdleRotateDir = 1;

	public static ShipController GetHuman()
	{
		for (int i=0; i<ShipController.Ships.Count; i++)
		{
			if (ShipController.Ships[i].Pilot == PilotType.Human)
			{
				return ShipController.Ships[i];
			}
		}
		return null;
	}

	void Awake()
	{
		ShipController.Ships.Add(this);
	}

	void OnDestroy()
	{
		ShipController.Ships.Remove(this);
	}

	// Use this for initialization
	void Start() 
	{
		this.InitMessenger("ShipController");
		this.rb = this.gameObject.GetComponent<Rigidbody>();

		this.Leader.gameObject.transform.SetParent(null);
		this.Leader.SetSpeed(this.MoveSpeed);

		if (this.Rotator != null)
		{
			this.StartRotate = this.Rotator.transform.localRotation.z;
			this.CurrentRotate = this.StartRotate;
		}

		this.LoadWeapon(this.PrimaryWeapon);
	}

	public void LoadWeapon(ShipWeapon weapon)
	{
		if (weapon == null)
		{
			return;
		}

		weapon.LoadWeapon();

		if (this.Reticle != null)
		{
			this.Reticle.SetScale(weapon.GetReticleSize());
		}
	}

	void Update() 
	{
		if (this.Leader.transform.position == this.transform.position)
		{
			return;
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

		this.UpdateThrusters();
		this.UpdateRotator();
		this.UpdateWeapons();
		this.ScanForHostiles();
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

	private bool RotateTowards(float targetAngle, float speed)
	{
		this.CurrentRotate = Mathf.MoveTowards( this.CurrentRotate, targetAngle, speed*Time.deltaTime);
		this.Rotator.transform.localRotation = Quaternion.Euler(0f, 0f, this.CurrentRotate);
		return (targetAngle == this.CurrentRotate);
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
				bool done = this.RotateTowards(this.GoalRotate, this.IdleRotateSpeed);
				if (done) 
				{
					this.GoalRotate = 0f;
					this.RotateStarted = false;
				}
			}
			else 
			{
				bool done = this.RotateTowards(this.StartRotate, this.IdleRotateSpeed);
				if (done) 
				{
					float minRotate = 0.2f;
					float randomT = (this.NextIdleRotateDir > 0) ? Random.Range(minRotate, 1f) : Random.Range(-1f, -1*minRotate);
					float randomAngle = this.MaxIdleRotate*randomT;
					this.GoalRotate = randomAngle + this.StartRotate;
					this.RotateStarted = true;

					this.NextIdleRotateDir *= -1f;
				}
			}
		}
		else if (this.rotateMode == RotateMode.Turn)
		{
			if (this.RotateStarted)
			{
				bool done = this.RotateTowards(this.GoalRotate, this.RotateSpeed);
				if (done) 
				{
					this.GoalRotate = 0f;
					this.RotateStarted = false;
				}
			}
		}
		else if (this.rotateMode == RotateMode.SpiralTurn)
		{
		}
	}

	private void ScanForHostiles()
	{
		for (int i=0; i<ShipController.Ships.Count; i++)
		{
			ShipController ship = ShipController.Ships[i];
			if ((ship == null) || (ship == this))
			{
				continue;
			}

			if (!this.HostileShips.Contains(ship))
			{
				this.HostileShips.Add(ship);
			}
		}
	}

	private void UpdateWeapons()
	{
		if (this.PrimaryWeapon == null)
		{
			return;
		}

		for (int i=0; i<this.HostileShips.Count; i++)
		{
			ShipController hostile = this.HostileShips[i];
			if (hostile == null)
			{
				continue;
			}
			bool isFiring = this.IsFiring(this.PrimaryWeapon);
			bool inFireArc = this.IsInFireArc(this.PrimaryWeapon, hostile.gameObject);
			if (!isFiring && inFireArc)
			{
				this.FireAt(this.PrimaryWeapon, hostile.gameObject);
			}
			else if (isFiring && !inFireArc)
			{
				this.StopFiring(this.PrimaryWeapon);
			}
		}
	}

	public bool IsInFireArc(ShipWeapon weapon, GameObject target)
	{
		if (weapon == null)
		{
			return false;
		}

		return weapon.IsInFireArc(target, this.transform);
	}

	public void FireAt(ShipWeapon weapon, GameObject target)
	{
		if (weapon == null)
		{
			return;
		}

		weapon.FireAt(target);
	}

	public bool IsFiring(ShipWeapon weapon)
	{
		if (weapon == null)
		{
			return false;
		}

		return weapon.IsFiring();
	}

	public void StopFiring(ShipWeapon weapon)
	{
		if (weapon == null)
		{
			return;
		}

		weapon.StopFiring();
	}

	public Vector3 GetReticleSize()
	{
		return this.PrimaryWeapon.GetReticleSize();
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
