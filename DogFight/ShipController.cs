using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using DSTools;

namespace DogFight
{

public class ShipController : Combatant
{
	static public List<ShipController> Ships = new List<ShipController>();

	public enum RotateMode
	{
		Idle,
		Turn,
		SpiralTurn
	}
	public RotateMode rotateMode = RotateMode.Idle;

	public enum MoveMode
	{
		Idle,
		Launching,
		Warping,
		Coasting,
		FreeNav,
		SplineNav
	}
	public MoveMode moveMode = MoveMode.Idle;

	public GameObject [] FreeNavWaypoints;
	private int FreeNavWaypointIndex = -1;

	public float MoveSpeed = 1f;
	public float BoostSpeed = 1f;
	public float SlowSpeed = 1f;
	public float RotateSpeed = 1f;
	public float DesiredLeaderDist = 5f;

	public float IdleRotatorSpeed = 5f;
	public float IdleRotatorReturnSpeed = 30f;
	public float MaxIdleRotate = 20f;

	public float TurnRotatorSpeed = 40f;
	public float MaxTurnRotate = 20f;

	public ShipLeader Leader;
	public Seeker Seeker;
	public GameObject Rotator;

	public GameObject [] Thrusters;

	public float WarpDistance = 1000;
	public float WarpDuration = 3f;
	private float WarpTime = 3f;
	private Vector3 WarpFrom;
	private Vector3 WarpTo;

	private bool changingPath = false;
	private Vector3 pathMergePoint;

	public ArenaSpawn Spawn;

	public ShipReticle Reticle;

	public ShipWeapon PrimaryWeapon;

	private Rigidbody rb;

	public HudController HUD;

	public Animator [] HangerDoors;
	private bool hangerOpen = false;

	private Vector3 coastVector;
	private bool isCoasting = false;
	private float currentSpeed = 0f;
	private float boostAmount = 0f;

	public float StartRotate = 0f;
	public float GoalRotate = 0f;
	public float CurrentRotate = 0f;
	public float CurrentTurnY = 0f;

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

	public static void OpenAllHangers()
	{
		//Debug.LogError("OpenAllHangers ships=" + Ships.Count );
		for (int i=0; i<ShipController.Ships.Count; i++)
		{
			ShipController.Ships[i].OpenHanger();
		}
	}

	public static void LaunchAll()
	{
		//Debug.LogError("LaunchAll ships=" + Ships.Count );
		for (int i=0; i<ShipController.Ships.Count; i++)
		{
			ShipController.Ships[i].LaunchHanger();
		}
	}

	void Awake()
	{
		ShipController.Ships.Add(this);

		this.rb = this.gameObject.GetComponent<Rigidbody>();
	}

	void OnDestroy()
	{
		ShipController.Ships.Remove(this);
	}

	// Use this for initialization
	void Start() 
	{
		this.InitMessenger("ShipController");

		if (this.Rotator != null)
		{
			this.StartRotate = this.Rotator.transform.localRotation.z;
			this.CurrentRotate = this.StartRotate;
		}

		this.currentSpeed = 0f;
		this.boostAmount = 0f;
	}

	public bool IsLeaderEjected()
	{
		return (this.Leader.gameObject.transform.parent == null);
	}

	public void EjectLeader()
	{
		if (!IsLeaderEjected())
		{
			this.Leader.gameObject.name = this.gameObject.name + "_Leader";
			this.Leader.gameObject.transform.SetParent(null);
			this.UpdateMoveSpeed();
		}
	}

	private float GetDesiredSpeed()
	{
		if (this.boostAmount == 0f)
		{
			return this.MoveSpeed;
		}
		else if (this.boostAmount > 0f)
		{
			return Mathf.Lerp(this.MoveSpeed, this.BoostSpeed, this.boostAmount);
		}
		else
		{
			return Mathf.Lerp(this.MoveSpeed, this.SlowSpeed, -1f*this.boostAmount);
		}
	}

	private void UpdateMoveSpeed()
	{
		if (this.Leader != null)
		{
			this.Leader.SetSpeed(this.GetDesiredSpeed());
		}
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

	public override GameObject GetCameraLookAt()
	{
		foreach(Transform child in this.transform)
		{
        	if(child.tag == "CameraLookAt")
        	{
				return child.gameObject;
        	}
        }
        return null;
	}

	public override GameObject GetWeaponLookAt()
	{
		foreach(Transform child in this.Rotator.transform)
		{
        	if(child.tag == "WeaponLookAt")
        	{
				return child.gameObject;
        	}
        }
        return null;
	}


	public override void SetupCamera()
	{
		CameraManager.Instance.EnableFollowCam(this.gameObject, GetCameraLookAt());
	}

	public override void SetupHUD()
	{
		HudController hud = HudController.Instance;
		if (hud != null)
		{
			hud.CreateHud(this, this.GetWeaponLookAt());
		}
	}

	private IEnumerator CoastForSeconds(float secs) 
	{
		this.coastVector = this.transform.forward;
		MoveMode previousMode = this.moveMode;
		this.moveMode = MoveMode.Coasting;
		yield return new WaitForSeconds(secs);

		this.moveMode = previousMode;
 	}

	public void OpenHanger()
	{
		//Debug.LogError("OpenHanger " + this.name + " this.hangerOpen=" + this.hangerOpen);
		if (!this.hangerOpen)
		{
			for (int i=0; i<this.HangerDoors.Length; i++)
			{
				this.HangerDoors[i].SetBool("Open", true);
			}

			if (this.HangerDoors.Length > 0)
			{
				this.hangerOpen = true;
			}
		}
	}

	private void CloseHanger()
	{
		//Debug.LogError("CloseHanger this.hangerOpen=" + this.hangerOpen);
		if (this.hangerOpen)
		{
			for (int i=0; i<this.HangerDoors.Length; i++)
			{
				this.HangerDoors[i].SetBool("Open", false);
			}

			if (this.HangerDoors.Length > 0)
			{
				this.hangerOpen = false;
			}
		}
	}

	public void LaunchHanger()
	{
		//Debug.LogError("LaunchHanger");
		StartCoroutine(LaunchHangerInternal());
	}

	private IEnumerator LaunchHangerInternal()
	{
		//Debug.LogError("LaunchHangerInternal" + this.name);

		if (this.Rotator != null)
		{
			if (!this.hangerOpen)
			{
				this.OpenHanger();

				//Wait for hanger door animation to finish
				yield return new WaitForSeconds(2f);
			}

			ShipController [] ships = this.Rotator.GetComponentsInChildren<ShipController>();
			//Debug.LogError("LaunchHanger ships=" + ships.Length);
			for (int i=0; i<ships.Length; i++)
			{
				ShipController ship = ships[i];
				ship.gameObject.transform.SetParent(null);
				ship.Launch();

				yield return new WaitForSeconds(2f);
			}

			//Wait to close hanger door
			yield return new WaitForSeconds(4f);
			this.CloseHanger();
		}
	}

	public void Launch() 
	{
		StartCoroutine(Launch(0.5f));
	}

	public void Warp(Transform to) 
	{
		if (this.transform.parent != null)
		{
			this.transform.SetParent(null);
		}

		this.UpdateMoveSpeed();

		this.moveMode = MoveMode.Warping;
		this.WarpTo = to.position;
		this.WarpFrom = this.WarpTo - this.WarpDistance * to.transform.forward;
		this.WarpTime = 0f;

		this.transform.position = this.WarpFrom;
		this.transform.rotation = this.transform.rotation;
	}

	public SplineGroup GetSplineGroup() 
	{
		if (this.Leader != null)
		{
			return this.Leader.splineGroup;
		}
		return null;
	}

	public void SetSplineGroup(SplineGroup splineGroup) 
	{
		if (this.Leader != null)
		{
			this.Leader.SetSplineGroup(splineGroup);
		}
	}

	private IEnumerator EnableForBattleIn(float secs)
	{
		EjectLeader();
		this.moveMode = MoveMode.SplineNav;

		SplineGroup splineGroup = SplineGroup.GetNewGroup(this.Leader);
		this.SetSplineGroup(splineGroup);

		if (secs > 0f) yield return new WaitForSeconds(secs);

		if (this.Pilot == PilotType.Human)
		{
			SetupCamera();
			SetupHUD();
		}

		this.LoadWeapon(this.PrimaryWeapon);
	}

	public void EnableForBattle()
	{
		StartCoroutine(EnableForBattleIn(0f));
	}

	public void EnableForFreeNav()
	{
		EjectLeader();
		this.moveMode = MoveMode.FreeNav;
	}

	private IEnumerator Launch(float coastSecs) 
	{
		this.currentSpeed = this.GetDesiredSpeed();
		this.UpdateMoveSpeed();
		yield return StartCoroutine( CoastForSeconds(coastSecs) );

		if (this.FreeNavWaypoints.Length > 0)
		{
			EnableForFreeNav();
		}
		else 
		{
			EnableForBattle();
		}
	}

	void Update() 
	{
		if (this.IsDead())
		{
			this.StopFiring(this.LoadedWeapon);
			this.Stop();
			this.UpdateThrusters();
			float deadSpeed = 0.2f * this.GetDesiredSpeed();
			this.transform.position += this.transform.forward * deadSpeed * Time.deltaTime;
			return;
		}

		if (this.moveMode == MoveMode.Warping)
		{
			this.WarpTime += Time.deltaTime;

			float t = this.WarpTime / this.WarpDuration;
			t = Mathf.Min(t*t*t*t, 1f);

			this.transform.position = Vector3.Lerp(this.WarpFrom, this.WarpTo, t);

			this.UpdateThrusters();

			return;
		}
		else if (this.moveMode == MoveMode.Coasting)
		{
			this.currentSpeed *= 1.02f;
			this.transform.position += this.coastVector * this.currentSpeed * Time.deltaTime;

			this.UpdateThrusters();
			return;
		}

		if (!this.IsLeaderEjected())
		{
			return;
		}

		Vector3 targetPosition = this.Leader.GetTargetPosition();
		Vector3 desiredDirection = (targetPosition - this.transform.position).normalized;
		Quaternion desiredRotation = (desiredDirection != Vector3.zero) ? Quaternion.LookRotation(desiredDirection) : this.transform.rotation;

		if (this.moveMode == MoveMode.SplineNav)
		{
			if (this.Leader.IsAtDestination())
			{
				this.Stop();
				this.StartCoroutine( this.CoastForSeconds(1.5f) );
				return;
			}

			if (!this.IsMoving())
			{
				this.Leader.MoveSpline();
				return;
			}
		}
		else if (this.moveMode == MoveMode.FreeNav)
		{
			if (this.Leader.IsAtDestination())
			{
				this.Stop();
				return;
			}

			if (!this.IsMoving())
			{
				if ((this.FreeNavWaypointIndex >= this.FreeNavWaypoints.Length) || (this.FreeNavWaypointIndex < 0))
				{
					this.FreeNavWaypointIndex = 0;
				}
				else
				{
					this.FreeNavWaypointIndex++;
				}

				GameObject nextPoint = this.FreeNavWaypoints[this.FreeNavWaypointIndex];
				this.MoveToFreeNav(nextPoint.transform.position);
			}
		}

		if (targetPosition == this.transform.position)
		{
			return;
		}

		Vector3 cross = Vector3.Cross(this.transform.rotation*Vector3.forward, desiredRotation*Vector3.forward);
		this.CurrentTurnY = cross.y;

		this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, desiredRotation, this.RotateSpeed*Time.deltaTime);
		if (!this.IsTurning())
		{
			this.SetRotateMode(RotateMode.Idle);
		}
		else
		{
			this.SetRotateMode(RotateMode.Turn);
		}

		float distanceToLeader = Vector3.Distance(this.transform.position, this.Leader.GetTargetPosition());
		this.currentSpeed = this.GetDesiredSpeed() * Mathf.Min(distanceToLeader/this.DesiredLeaderDist, 2.0f);
		this.transform.position += this.transform.forward * this.currentSpeed * Time.deltaTime;

		if (this.Leader.IsMoving() && (distanceToLeader > 1.5f*this.currentSpeed))
		{
			this.Leader.Pause(0.01f);
		}
		else
		{
			this.Leader.UnPause();
		}

		this.UpdateMoveSpeed();
		this.UpdateThrusters();
		this.UpdateRotator();
		this.UpdateWeapons();
		this.ScanForHostiles();
	}

	private void UpdateThrusters()
	{
		for (int i=0; i<this.Thrusters.Length; i++)
		{
			TrailRenderer trail = this.Thrusters[i].GetComponent<TrailRenderer>();
			if (this.moveMode == MoveMode.Warping)
			{
				trail.time = 0.1f;
			}
			else if (this.IsDead())
			{
				trail.time = 0.0f;
			}
			else
			{
				trail.time = 0.3f;
			}
		}
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

	public bool IsTurning()
	{
		float absY = Mathf.Abs(this.CurrentTurnY);
		return (absY > 0.0035f);
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
				bool done = this.RotateTowards(this.GoalRotate, this.IdleRotatorSpeed);
				if (done) 
				{
					this.GoalRotate = 0f;
					this.RotateStarted = false;
				}
			}
			else 
			{
				bool done = this.RotateTowards(this.StartRotate, this.IdleRotatorReturnSpeed);
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
				this.GoalRotate = this.CurrentTurnY * -5000f;
				this.GoalRotate = Mathf.Clamp(this.GoalRotate, -1f*this.MaxTurnRotate, this.MaxTurnRotate);

				bool done = this.RotateTowards(this.GoalRotate, this.TurnRotatorSpeed);
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

	public float GetSpeed()
	{
		return currentSpeed;
	}

	public bool IsMoving()
	{
		return this.Leader.IsMoving();
	}

	public void MoveToFreeNav(Vector3 to)
	{
		StartMoveFreeNav(this.Leader.GetTargetPosition(), to);
	}

	public void MovePathFreeNav(Vector3 from, Vector3 to)
	{
		StartMoveFreeNav(from, to);
	}

	private void StartMoveFreeNav(Vector3 from, Vector3 to)
	{
		if (this.isPathfinding)
		{
			return;
		}
		this.isPathfinding = true;

		Debug.LogError("StartMove " + from.ToString() + " -> " + to.ToString());
		this.Seeker.StartPath(from, to, OnMovePathComplete);
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

	public void AdjustPosition(Vector2 adjustTargetPos)
	{
		float maxRadius = 30f;
		Vector3 pos = this.Rotator.transform.localPosition;
		pos.x += adjustTargetPos.x;
		pos.y += adjustTargetPos.y;
		pos.z = 0f;

		this.Rotator.transform.localPosition = Vector3.ClampMagnitude(pos, maxRadius);
	}

	public override void HandleTransform(Vector2 deltaPos)
	{
		if (this.IsMoving())
		{
			if (this.moveMode == MoveMode.SplineNav)
			{
				Vector3 adjustment = 0.1f*deltaPos;
				this.AdjustPosition(adjustment);
			}
		}
	}

	public override void HandleThrottle(float throttleAmount)
	{
		this.boostAmount = throttleAmount;
	}

	public override void HandleFlick(Vector2 flickVector)
	{
	}

	void OnCollisionEnter(Collision info)
	{
		//GameObject obj = info.collider.gameObject;
	}
}

}
