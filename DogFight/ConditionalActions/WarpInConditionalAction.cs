using UnityEngine;
using System.Collections;

namespace DogFight
{

public class WarpInConditionalAction : DSTools.ConditionalAction
{
	public ShipController Ship;
	public Transform WarpTo;

	protected override void OnConditionsMet()
	{
		if (this.Ship != null)
		{
			this.Ship.Warp(WarpTo);
		}
	}
}

}
