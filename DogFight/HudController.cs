using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DogFight
{

public class HudController : MonoBehaviour
{
	public GameObject ReticlePrefab;
	public GameObject TargetPrefab;

	public List<SpriteObjectTracker> Targets;

	public void CreateHud(ShipController humanShip, GameObject lookAt)
	{
		GameObject reticle = GameObject.Instantiate(this.ReticlePrefab);
		SpriteObjectTracker tracker = reticle.GetComponent<SpriteObjectTracker>();
		if (tracker == null)
		{
			GameObject.Destroy(reticle);
			return;
		}
		reticle.transform.SetParent(this.transform);
		tracker.target = lookAt;
		humanShip.Reticle = tracker;
	}

	public void CreateTargetable(ShipController ship)
	{
		GameObject reticle = GameObject.Instantiate(this.TargetPrefab);
		SpriteObjectTracker tracker = reticle.GetComponent<SpriteObjectTracker>();
		if (tracker == null)
		{
			GameObject.Destroy(reticle);
			return;
		}
		reticle.transform.SetParent(this.transform);
		tracker.target = ship.gameObject;
		this.Targets.Add(tracker);
	}

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		ShipController human = ShipController.GetHuman();
		if (human == null)
		{
			return;
		}

		for (int i=0; i<this.Targets.Count; i++)
		{
			this.Targets[i].viewPosition = human.gameObject;
		}
	}
}

}

