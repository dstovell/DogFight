using UnityEngine;
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
	public GameObject DeathFx;
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
			//Debug.LogError(this.transform.root.gameObject.name + " is dead!");
			this.StartCoroutine(this.OnDeath());
		}
	}

	private IEnumerator OnDeath() 
	{
		if (this.DeathFx != null)
		{
			this.DeathFx.SetActive(true);
			this.DeathFx.transform.localScale = 10f*Vector3.one;
		}

		if (this.DestroyOnDeath)
		{
			yield return new WaitForSeconds(this.DestroyWaitSeconds);
			this.gameObject.SetActive(false);
		}
 	}

	public void OnTriggerEnter(Collider other)
	{
		//Debug.LogError("OnTriggerEnter " + this.transform.root.gameObject.name + " hit " + other.gameObject.name );
	}
}

}
