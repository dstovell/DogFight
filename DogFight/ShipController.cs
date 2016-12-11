﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using DSTools;

namespace DogFight
{

public class AvailableMove
{
	public AvailableMove(Vector3 pos, Vector3 conPos, Vector2 proj)
	{
		this.position = pos;	
		this.projection = proj;
		this.connectionPosition = conPos;
	}

	public float AngleBetween(Vector2 a)
	{
		return Vector2.Angle(this.projection, a);
	}

	public Vector3 position;
	public Vector2 projection;
	public Vector3 connectionPosition;
}

public class ShipController : MessengerListener
{
	static public List<ShipController> Ships = new List<ShipController>();

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

	public GameObject [] Thrusters;

	private Vector3 currentMoveFrom;
	private Vector3 currentMoveTo;

	private bool changingPath = false;
	private Vector3 pathMergePoint;

	public ArenaSpawn Spawn;

	public bool AutoFiring = false;

	public SpriteObjectTracker Reticle;

	public ShipWeapon LoadedWeapon;

	public ShipWeapon PrimaryWeapon;

	public List<ShipController> HostileShips = new List<ShipController>();

	private Damageable damagable;
	private Rigidbody rb;

	private Vector3 coastVector;
	private bool isCoasting = false;
	private float currentSpeed = 0f;

	public float StartRotate = 0f;
	public float GoalRotate = 0f;
	public float CurrentRotate = 0f;

	public bool RotateStarted = false;
	private float NextIdleRotateDir = 1;

	private bool isPathfinding = false;

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

		this.rb = this.gameObject.GetComponent<Rigidbody>();
		this.damagable = this.gameObject.GetComponent<Damageable>();
	}

	void OnDestroy()
	{
		ShipController.Ships.Remove(this);
	}

	// Use this for initialization
	void Start() 
	{
		this.InitMessenger("ShipController");

		this.Leader.gameObject.name = this.gameObject.name + "_Leader";
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
		this.LoadedWeapon = weapon;
	}

	private IEnumerator CoastForSeconds(float secs) 
	{
		this.coastVector = this.transform.forward;
		this.isCoasting = true;
		yield return new WaitForSeconds(secs);

		this.isCoasting = false;
 	}

	void Update() 
	{
		if (this.IsDead())
		{
			this.StopFiring(this.LoadedWeapon);
			this.Stop();
			float deadSpeed = 0.2f * this.MoveSpeed;
			this.transform.position += this.transform.forward * deadSpeed * Time.deltaTime;
			return;
		}

		Vector3 desiredDirection = (this.Leader.transform.position - this.transform.position).normalized;
		Quaternion desiredRotation = Quaternion.LookRotation(desiredDirection);

		if (this.isCoasting)
		{
			this.currentSpeed *= 1.02f;
			this.transform.position += this.coastVector * this.currentSpeed * Time.deltaTime;

			this.UpdateThrusters();
			return;
		}

		if (this.Leader.IsAtDestination())
		{
			this.Stop();
			Vector3 otherSide = this.Spawn.GetFurthestEnd(this.Leader.transform.position);
			this.currentMoveFrom = this.Leader.transform.position;
			this.currentMoveTo = otherSide;
			this.StartCoroutine( this.CoastForSeconds(1.5f) );
			return;
		}

		if (!this.IsMoving())
		{
			this.ContinueCurrentMove();
			return;
		}

		if (this.Leader.transform.position == this.transform.position)
		{
			return;
		}

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

		if (this.Leader.IsMoving() && (distanceToLeader > 1.5f*this.currentSpeed))
		{
			this.Leader.Pause(0.01f);
		}

		this.UpdateThrusters();
		this.UpdateRotator();
		this.UpdateWeapons();
		this.ScanForHostiles();
	}

	private void UpdateThrusters()
	{
//		for (int i=0; i<this.Thrusters.Length; i++)
//		{
//			TrailRenderer trail = this.Thrusters[i].GetComponent<TrailRenderer>();
//			if (this.isCoasting)
//			{
//				trail.time = Mathf.Max(0.0f, trail.time-0.01f);
//			}
//			else
//			{
//				trail.time = Mathf.Min(0.5f, trail.time+0.01f);
//			}
//		}
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
		if (this.LoadedWeapon == null)
		{
			return;
		}

		if (this.AutoFiring)
		{
			for (int i=0; i<this.HostileShips.Count; i++)
			{
				ShipController hostile = this.HostileShips[i];
				if (hostile == null)
				{
					continue;
				}
				bool isFiring = this.IsFiring(this.LoadedWeapon);
				bool inFireArc = this.IsInFireArc(this.LoadedWeapon, hostile.gameObject);
				if (!isFiring && inFireArc)
				{
					this.FireAt(this.LoadedWeapon, hostile.gameObject);
				}
				else if (isFiring && !inFireArc)
				{
					this.StopFiring(this.LoadedWeapon);
				}
			}
		}
	}

	public List<AvailableMove> GetAvailableMoves()
	{
		float minDistance = 100;
		List<AvailableMove> moves = new List<AvailableMove>();
		NNInfo nextNodeInfo = this.GetNextNode(minDistance);
		if (nextNodeInfo.node != null)
		{
			PointNode nextNode = nextNodeInfo.node as PointNode;
			Vector3 nextNodePos = (Vector3)nextNode.position;
			if (nextNode != null)
			{
				for (int i=0; i<nextNode.connections.Length; i++)
				{
					GraphNode moveNode = nextNode.connections[i];
					Vector3 pos = (Vector3)moveNode.position;
					if (this.Leader.IsOnCurrentPath(pos))
					{
						continue;
					}

					if (this.transform.forward.z > 0)
					{
						if (pos.z > this.transform.position.z)
						{
							moves.Add(new AvailableMove(pos, nextNodePos, this.ProjectPoint(pos)));
						}
					}
					else
					{
						if (pos.z < this.transform.position.z)
						{
							moves.Add(new AvailableMove(pos, nextNodePos, this.ProjectPoint(pos)));
						}
					}
				}
			}
		}
		return moves;
	}

	public Vector2 ProjectPoint(Vector3 pos)
	{
		Vector3 currentPos = this.transform.position;
		Vector2 proj = new Vector2(pos.x - currentPos.x, pos.y - currentPos.y);
		return proj.normalized;
	}

	public NNInfo GetNextNode(float minDistance)
	{
		Vector3 waypointPos = this.Leader.GetNextWayPoint(minDistance);
		if (waypointPos == Vector3.zero)
		{
			NNInfo nni = new NNInfo();
			return nni;
		}
		NNInfo node = AstarPath.active.astarData.pointGraph.GetNearest(waypointPos);
		return node;
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

	public void FireAt(Vector2 screenPoint)
	{
		if (this.LoadedWeapon == null)
		{
			return;
		}

		float screenDistance = 9999f;
		GameObject target = null;
		for (int i=0; i<this.HostileShips.Count; i++)
		{
			ShipController hostile = this.HostileShips[i];
			if (hostile == null)
			{
				continue;
			}
			bool inFireArc = this.IsInFireArc(this.LoadedWeapon, hostile.gameObject);
			if (inFireArc)
			{
				Vector3 screenPos = Camera.main.WorldToViewportPoint(hostile.transform.position);
				float dist = Vector3.Distance(screenPos, screenPoint);
				if (dist < screenDistance)
				{
					target = hostile.gameObject;
				}
			}
		}

		this.LoadedWeapon.FireAt(target);
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
		return (this.LoadedWeapon != null) ? this.LoadedWeapon.GetReticleSize() : Vector3.zero;
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

	public bool IsDead()
	{
		return this.damagable.IsDead();
	}

	public void MoveTo(Vector3 to)
	{
		if (this.isPathfinding)
		{
			return;
		}
		this.isPathfinding = true;

		this.currentMoveFrom = this.Leader.transform.position;
		this.currentMoveTo = to;

		this.ContinueCurrentMove();
	}

	public void MovePath(Vector3 from, Vector3 to)
	{
		if (this.isPathfinding)
		{
			return;
		}
		this.isPathfinding = true;

		this.currentMoveFrom = from;
		this.currentMoveTo = to;

		this.ContinueCurrentMove();
	}

	private void ContinueCurrentMove()
	{
		this.Seeker.StartPath(this.currentMoveFrom, this.currentMoveTo, OnMovePathComplete);
	}

	private void ChangePath(Vector3 connectionPosition, Vector3 pathStart)
	{
		this.pathMergePoint = connectionPosition;
		this.changingPath = true;

		this.currentMoveFrom = pathStart;
		this.ContinueCurrentMove();
	}

	private void OnMovePathComplete(Path p) 
	{
		p.Claim(this);

		List<Vector3> newPath = p.vectorPath;

		if (this.changingPath)
		{
			newPath = new List<Vector3>();
			Vector3 [] currentPath = this.Leader.mover.waypoints;
			int startIndex = this.Leader.mover.currentPoint+1;
			for (int i=startIndex; i<currentPath.Length; i++)
			{
				if (currentPath[i+1] == this.pathMergePoint)
				{
					break;
				}

				newPath.Add(currentPath[i]);
			}

			for (int j=0; j<p.vectorPath.Count; j++)
			{
				newPath.Add(p.vectorPath[j]);
			}

			this.changingPath = false;
		}

		this.MovePath(newPath);

		//ArenaManager.Instance.HighlightPath(p.vectorPath);

		this.isPathfinding = false;
	}

	public void MovePath(List<Vector3> points)
	{
		this.Leader.MovePath(points);
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

	private void HandleFlick(Vector2 flickVector)
	{
		float minThreshold = 10;	

		flickVector.Normalize();
		List<AvailableMove> moves = this.GetAvailableMoves();
		float closestAngle = 400;
		AvailableMove closestMove = null;
		for (int i=0; i<moves.Count; i++)
		{
			float angleBetween = moves[i].AngleBetween(flickVector);
			if (angleBetween < closestAngle)
			{
				closestAngle = angleBetween;
				closestMove = moves[i];
			}
		}

		if (closestAngle <= minThreshold)
		{
			Debug.Log("flickVector " + flickVector.ToString());
			Debug.Log("closestMove proj" + closestMove.projection.ToString());
			Debug.Log("closestAngle " + closestAngle);
			Debug.Log("closestMove " + closestMove.position.ToString());

			this.ChangePath(closestMove.connectionPosition, closestMove.position);
		}
	}

	void OnCollisionEnter(Collision info)
	{
		//GameObject obj = info.collider.gameObject;
	}

	public override void OnMessage(string id, object obj1, object obj2)
	{
		if (this.Pilot != PilotType.Human)
		{
			return;
		}

		switch(id)
		{
		case "tap":
		{
			Vector2 screenPoint = (Vector2)obj1;
			if (!this.AutoFiring)
			{
				this.FireAt(screenPoint);
			}
		}
		break;

		case "flick":
		{
			Vector2 flickVector = (Vector2)obj1;
			this.HandleFlick(flickVector);
		}
		break;

		default:
			break;
		}
	}
}

}
