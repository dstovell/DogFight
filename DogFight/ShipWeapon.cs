﻿using UnityEngine;
using System.Collections;
using Forge3D;

namespace DogFight
{

public class ShipWeapon : MonoBehaviour 
{
	public WeaponFXController FX;

	public Forge3D.F3DFXType FxType;
	public float BurstTime;
	public float FireCooldown;
	public float Damage;
	public float MaxRange;
	public float FireArcDegrees;
	public LayerMask DamageMask;

	public Transform [] Barrels;

	public GameObject [] Reticles;

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

		for (int i=0; i<this.Reticles.Length; i++)
		{
			this.Reticles[i].SetActive(true);
		}
	}

	public bool IsFiring()
	{
		return this.isFiring;
	}

	public void FireAt(GameObject target = null)
	{
		if (this.IsFiring() || this.IsOnCooldown())
		{
			return;
		}

		if (target != null)
		{
			for (int i=0; i<this.Barrels.Length; i++)
			{
				Transform barrel = this.Barrels[i];
				if (barrel != null)
				{
					if (target != null)
					{
						barrel.rotation = Quaternion.LookRotation( (target.transform.position - barrel.position).normalized );
					}
					else
					{
						barrel.localRotation = Quaternion.identity;
					}
				}
			}
		}

		this.FX.Fire(this.DamageMask, this.Damage);
		this.isFiring = true;

//		if (target != null)
//		{
//			this.OnHit(target);
//		}

		if (this.BurstTime > 0.0f)
		{
			float burstTime = Mathf.Max(this.BurstTime, 0.1f);
			StartCoroutine( StopFiringInSeconds(burstTime) );
		}
	}

	public void StopFiring()
	{
		this.currentCooldown = this.FireCooldown;
		this.isFiring = false;
		this.FX.Stop();
	}

	private IEnumerator StopFiringInSeconds(float seconds) 
	{
		yield return new WaitForSeconds(seconds);
		this.StopFiring();
 	}

	public void OnHit(GameObject target)
	{
		Damageable dam = target.GetComponent<Damageable>();
		if (dam != null)
		{
			OnHit(dam);
			return;
		}

		Damageable parentDam = target.GetComponentInParent<Damageable>();
		if (parentDam != null)
		{
			OnHit(parentDam);
			return;
		}
	}

	public void OnHit(Damageable target)
	{
		if (target != null)
		{
			target.Damage(this.Damage);
		}
	}
}

}
