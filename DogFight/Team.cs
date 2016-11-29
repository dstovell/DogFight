using UnityEngine;
using System.Collections;

namespace DogFight
{

public class Team : MonoBehaviour
{
	public GameObject Human;
	public GameObject [] AIs;

	public HudController HUD;

	void Start()
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

	void Update()
	{
	
	}

	public bool IsHumanTeam()
	{
		return (this.Human != null);
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

		if (isHuman)
		{
			GameObject lookAt = null;
			foreach(Transform child in obj.transform)
			{
            	if(child.tag == "CameraLookAt")
            	{
					lookAt = child.gameObject;
					break;
            	}
            }

			KGFOrbitCam orbitCam = Camera.main.GetComponent<KGFOrbitCam>();
			if (orbitCam != null)
			{
				orbitCam.SetTarget(obj);
				if (lookAt != null)
				{
					orbitCam.SetLookatTarget(lookAt);
				}
			}

			if (this.HUD != null)
			{
				this.HUD.CreateHud(ship, lookAt);
			}
		}
		else if (!this.IsHumanTeam())
		{
			if (this.HUD != null)
			{
				this.HUD.CreateTargetable(ship);
			}
		}

		Arena.Instance.AssignSpawn(ship);
	}
}

}
