using UnityEngine;
using System.Collections;

namespace DogFight
{

public class IsDeadCondition : DSTools.Condition
{
	public Damageable [] Damagables;

	public override bool IsConditionMet()
	{
		bool allDead = true;
		for (int i=0; i<this.Damagables.Length; i++)
		{
			Damageable d = this.Damagables[i];
			if ((d != null) && !d.IsDead())
			{
				allDead = false;
				break;
			}
		}
		return allDead;
	}
}

}
