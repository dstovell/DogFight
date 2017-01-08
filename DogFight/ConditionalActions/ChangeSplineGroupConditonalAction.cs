using UnityEngine;
using System.Collections;

namespace DogFight
{

public class ChangeSplineGroupConditonalAction : DSTools.ConditionalAction
{
	public SplineGroup From;
	public SplineGroup To;

	public ShipController [] Ships;

	protected override bool OnConditionsMet(GameObject triggerer)
	{
		for (int i=0; i<this.Ships.Length; i++)
		{
			ShipController ship = this.Ships[i];
			if ((ship != null) && (ship.gameObject == triggerer))
			{
				SplineGroup currentGroup = ship.GetSplineGroup();
				if ((this.From == null) || (currentGroup == null) || (this.From == currentGroup))
				{
					if (currentGroup != this.To)
					{
						ship.SetSplineGroup(this.To);
						return true;
					}
				}
			}
		}
		return false;
	}
}

}
