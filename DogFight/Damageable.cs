﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DogFight
{

public class Damageable : MonoBehaviour
{
	static public List<Damageable> Damageables = new List<Damageable>();

	public enum Type
	{
		Hull,
		Bridge,
		Turret,
		Hanger,
		Engine
	}
	public Type ComponentType = Type.Hull;

	private bool isAlive = true;

	public float Health = 1f;
	public GameObject DeathFxPrefab;
	public bool DestroyOnDeath = true;
	public float DestroyWaitSeconds = 0f;

	void Awake()
	{
		Damageable.Damageables.Add(this);
	}

	void OnDestroy()
	{
		Damageable.Damageables.Remove(this);
	}

	public FactionType GetFaction()
	{
		Combatant combat = this.transform.root.gameObject.GetComponent<Combatant>();
		return (combat != null) ? combat.Faction : FactionType.Alliance;
	}

	public bool IsDead()
	{
		return !this.isAlive;
	}

	public void Damage(float amount)
	{
		if (!this.isAlive)
		{
			return;
		}

		this.Health = Mathf.Max(0f, this.Health - amount);
		if (this.Health == 0)
		{
			this.isAlive = false;
		}

		if (!this.isAlive)
		{
			Debug.LogError(this.gameObject.name + " is dead!");
			this.StartCoroutine(this.OnDeath());
		}
	}

	private IEnumerator OnDeath() 
	{
		if (this.DeathFxPrefab != null)
		{
			GameObject obj = GameObject.Instantiate(this.DeathFxPrefab, this.transform.position, this.transform.rotation) as GameObject;
			obj.transform.SetParent(this.transform);
		}

		if (this.DestroyOnDeath)
		{
			yield return new WaitForSeconds(this.DestroyWaitSeconds);
			GameObject.Destroy(this.gameObject);
		}
 	}

	public void OnTriggerEnter(Collider other)
	{
		Debug.LogError("OnTriggerEnter " + this.transform.parent.gameObject.name + " hit " + other.gameObject.name );
	}
}

}
