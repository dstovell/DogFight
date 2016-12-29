using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DogFight
{


public class SplineGroup : MonoBehaviour
{
	private static List<SplineGroup> groups = new List<SplineGroup>();

	public static SplineGroup GetNewGroup(ShipLeader ship)
	{
		for (int i=0; i<SplineGroup.groups.Count; i++)
		{
			return SplineGroup.groups[i];
		}
		return null;
	}

	public FluffyUnderware.Curvy.CurvySpline [] splines;

	void Awake()
	{
		SplineGroup.groups.Add(this);
	}

	void OnDestroy()
	{
		SplineGroup.groups.Remove(this);
	}
	
	void Update()
	{
	
	}

	public FluffyUnderware.Curvy.CurvySpline GetClosestSpline(Vector3 pos)
	{
		return splines[0];
	}

	public List<Vector3> GetChangeSplineDirections(FluffyUnderware.Curvy.CurvySpline refSpine, float refT)
	{
		List<Vector3> dirs = new List<Vector3>();

		Vector3 refPoint = refSpine.InterpolateFast(refT);
		for (int i=0; i<this.splines.Length; i++)
		{
			if (this.splines[i] == refSpine)
			{
				continue;
			}

			Vector3 splinePoint = this.splines[i].InterpolateFast(refT);
			Vector3 slineDir = (splinePoint - refPoint).normalized;
			dirs.Add(slineDir);
		}

		return dirs;
	}

	public FluffyUnderware.Curvy.CurvySpline GetChangeSpline(FluffyUnderware.Curvy.CurvySpline refSpine, float refT, Vector3 changeDirection, float maxAngle = 360)
	{
		List<Vector3> dirs = this.GetChangeSplineDirections(refSpine, refT);
		float closestAngle = 9999f;
		FluffyUnderware.Curvy.CurvySpline closestSpline = null;

		Vector3 refPoint = refSpine.InterpolateFast(refT);

		for (int i=0; i<this.splines.Length; i++)
		{
			if (this.splines[i] == refSpine)
			{
				continue;
			}

			Vector3 splinePoint = this.splines[i].InterpolateFast(refT);
			Vector3 splineDir = (splinePoint - refPoint).normalized;

			float angle = Vector3.Angle(splineDir, changeDirection);
			if (angle < closestAngle)
			{
				closestAngle = angle;	
				closestSpline = this.splines[i];
			}
		}

		return (closestAngle <= maxAngle) ? closestSpline : null;
	}
}


}
