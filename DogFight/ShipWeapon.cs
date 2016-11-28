using UnityEngine;
using System.Collections;
using Forge3D;

namespace DogFight
{

public class ShipWeapon : MonoBehaviour 
{
	public WeaponFXController FX;

	public Forge3D.F3DFXType FxType;
	public float FireCooldown;
	public float Damage;
	public float MaxRange;
	public float FireArcDegrees;

	public Transform [] Barrels;

	private float currentCooldown = 0f;
	private bool isFiring = false;

	void Awake() 
	{
		
	}

	void Start() 
	{
			
	}
	
	void Update() 
	{
		this.currentCooldown -= Time.deltaTime;
		this.currentCooldown = Mathf.Max(this.currentCooldown, 0f);
	}

	public Vector3 GetReticleSize()
	{
		float size = this.FireArcDegrees / 15f;
		return new Vector3(size, size, size);
	}

	public bool IsInFireArc(GameObject target, Transform referencePoint)
	{
		float targetDistance = Vector3.Distance(target.transform.position, referencePoint.position);
		if (targetDistance > this.MaxRange)
		{
			return false;
		}

		Vector3 targetDir = target.transform.position - referencePoint.position;

		float angle = Vector3.Angle(targetDir, referencePoint.forward);
		if (angle > 0.5f*this.FireArcDegrees)
		{
			return false;
		}

		return true;
	}

	public bool IsOnCooldown()
	{
		return (this.currentCooldown > 0f);
	}

	public void LoadWeapon()
	{
		this.FX.DefaultFXType = this.FxType;
		this.FX.TurretSocket = this.Barrels;
	}

	public bool IsFiring()
	{
		return this.isFiring;
	}

	public void FireAt(GameObject target)
	{
		if (this.IsFiring() || this.IsOnCooldown())
		{
			return;
		}

		this.FX.Fire();
		this.isFiring = true;

		this.currentCooldown = this.FireCooldown;
	}

	public void StopFiring()
	{
		this.isFiring = false;
		this.FX.Stop();
	}

	public void OnHit(GameObject target)
	{
		
	}
}

}
