using UnityEngine;
using System.Collections;

namespace DogFight
{

public class LaunchHangerConditionalAction : DSTools.ConditionalAction
{
	public ShipController [] Ships;

	protected override bool OnConditionsMet(GameObject triggerer)
	{
		for (int i=0; i<this.Ships.Length; i++)
		{
			if (this.Ships[i] != null)
			{
				this.Ships[i].LaunchHanger();
			}
		}
		return true;
	}
}

}
