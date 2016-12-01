using UnityEngine;
using System.Collections;

namespace DogFight
{

public class Damageable : MonoBehaviour
{
	private bool isAlive = true;

	public float Health = 1f;
	public GameObject DeathFxPrefab;
	public bool DestroyOnDeath = true;
	public float DestroyWaitSeconds = 0f;

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
}

}
