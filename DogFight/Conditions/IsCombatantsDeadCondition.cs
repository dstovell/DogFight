using UnityEngine;
using System.Collections;

namespace DogFight
{

public class IsCombatantDeadCondition : DSTools.Condition
{
	public Combatant [] Combatants;

	public override bool IsConditionMet()
	{
		bool allDead = true;
			for (int i=0; i<this.Combatants.Length; i++)
		{
			Combatant d = this.Combatants[i];
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
