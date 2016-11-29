using UnityEngine;
using System.Collections;

namespace DogFight
{

public class HudController : MonoBehaviour
{
	public GameObject ReticlePrefab;
	public GameObject TargetPrefab;

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
	}

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

}

