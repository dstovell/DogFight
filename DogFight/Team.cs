using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DogFight
{

public class Team : MonoBehaviour
{
	static public List<Team> Teams = new List<Team>();

	public GameObject Human;
	public GameObject [] AIs;

	public HudController HUD;

	void Awake()
	{
		Team.Teams.Add(this);
	}

	void OnDestroy()
	{
		Team.Teams.Remove(this);
	}

	void Update()
	{
	
	}

	public bool IsHumanTeam()
	{
		return (this.Human != null);
	}

	public void SpawnTeamMembers()
	{
		this.SpawnTeamMember(this.Human, true);

		if (this.AIs != null)
		{
			for (int i=0; i<this.AIs.Length; i++)
			{
				this.SpawnTeamMember(this.AIs[i]);
			}
		}
	}

	private void SpawnTeamMember(GameObject prefab, bool isHuman = false)
	{
		if (prefab == null)
		{
			return;
		}

		GameObject obj = GameObject.Instantiate(prefab) as GameObject;
		ShipController ship = obj.GetComponent<ShipController>(); 
		if (ship == null)
		{
			GameObject.Destroy(obj);
			return;
		}

		ship.Pilot = isHuman ? ShipController.PilotType.Human : ShipController.PilotType.AI;

		if (!this.IsHumanTeam())
		{
			if (this.HUD != null)
			{
				this.HUD.CreateTargetable(ship);
			}
		}

		AssignSplineGroup(ship);
	}

	public SplineGroup AssignSplineGroup(ShipController ship)
	{
		SplineGroup splineGroup = SplineGroup.GetNewGroup(ship.Leader);
		//Debug.LogError("AssignSplineGroup " + ship.Leader.name + " splineGroup=" + splineGroup);

		ship.Leader.SetSplineGroup(splineGroup);
		ship.Warp(ship.transform);

		return splineGroup;
	}
}

}
