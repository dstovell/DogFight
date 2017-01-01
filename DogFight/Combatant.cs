using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DSTools;

namespace DogFight
{

public class Combatant : MessengerListener
{
	public enum PilotType
	{
		AI,
		Human
	}
	public PilotType Pilot = PilotType.AI;
	public FactionType Faction = FactionType.Alliance;

	public bool AutoFiring = false;

	public ShipWeapon LoadedWeapon;

	public Damageable [] damagables;

	public List<Damageable> Hostiles = new List<Damageable>();

	public bool IsDead()
	{
		for (int i=0; i<this.damagables.Length; i++)
		{
			if (!this.damagables[i].IsDead())
			{
				return false;
			}
		}
		return true;
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
		for (int i=0; i<this.Hostiles.Count; i++)
		{
			Damageable hostile = this.Hostiles[i];
			if ((hostile == null) || hostile.IsDead())
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

	protected void ScanForHostiles()
	{
		for (int i=0; i<Damageable.Damageables.Count; i++)
		{
			Damageable d = Damageable.Damageables[i];
			if ((d == null) || !FactionController.Instance.IsHostile(this.Faction, d.GetFaction()))
			{
				continue;
			}

			if (!this.Hostiles.Contains(d))
			{
				this.Hostiles.Add(d);
			}
		}
	}

	protected void UpdateWeapons()
	{
		if (this.LoadedWeapon == null)
		{
			return;
		}

		if (this.AutoFiring)
		{
			for (int i=0; i<this.Hostiles.Count; i++)
			{
				Damageable hostile = this.Hostiles[i];
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
}

}