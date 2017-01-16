using UnityEngine;
using System.Collections;
using Forge3D;

public class WeaponFXController : F3DFXController
{
	public override void OnProjectileHit(F3DProjectile projectile, GameObject obj, Vector3 hitPoint)
    {
		DogFight.Damageable damageable = obj.GetComponent<DogFight.Damageable>();
		Debug.LogError("WeaponFXController OnProjectileHit name=" + obj.name + " rootName=" + obj.transform.root.gameObject.name + " damageable=" + damageable);
		if (damageable != null)
		{
			damageable.Damage(20f);
		}
    }

	void OnGUI()
	{
	}
}

