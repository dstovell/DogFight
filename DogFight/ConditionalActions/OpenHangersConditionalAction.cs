using UnityEngine;
using System.Collections;

namespace DogFight
{

public class OpenHangersConditionalAction : DSTools.ConditionalAction
{
	public ShipController [] Ships;

	protected override void OnConditionsMet()
	{
		for (int i=0; i<this.Ships.Length; i++)
		{
			if (this.Ships[i] != null)
			{
				this.Ships[i].OpenHanger();
			}
		}
	}
}

}
