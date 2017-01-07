using UnityEngine;
using System.Collections;

namespace DogFight
{

public class WarpInConditionalAction : DSTools.ConditionalAction
{
	public ShipController Ship;
	public Transform WarpTo;

	protected override bool OnConditionsMet(GameObject triggerer)
	{
		if (this.Ship != null)
		{
			this.Ship.Warp(WarpTo);
		}

		return true;
	}
}

}
