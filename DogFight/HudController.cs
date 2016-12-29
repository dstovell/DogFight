using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace DogFight
{

public class HudController : MonoBehaviour
{
	static public HudController Instance;

	public GameObject ReticlePrefab;
	public GameObject TargetPrefab;
	public GameObject DirectionPrefab;

	public List<SpriteObjectTracker> Targets;

	public List<Image> DirectionIndicators;

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

	void Awake()
	{
		HudController.Instance = this;
	}

	void Start()
	{
		this.DirectionIndicators = new List<Image>();
		int maxDirections = 10;
		if (this.DirectionPrefab != null)
		{
			for (int i=0; i<maxDirections; i++)
			{
				GameObject obj = GameObject.Instantiate(this.DirectionPrefab) as GameObject;
				Image img = obj.GetComponent<Image>();
				if (img == null)
				{
					GameObject.Destroy(obj);
				}
				else
				{
					img.enabled = false;
					this.DirectionIndicators.Add(img);
					obj.transform.SetParent(this.transform);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		ShipController human = ShipController.GetHuman();
		if ((human == null) || (human.Reticle == null))
		{
			return;
		}

		for (int i=0; i<this.Targets.Count; i++)
		{
			this.Targets[i].viewPosition = human.gameObject;
		}

		List<Vector3> moves = human.GetAvailableMoveDirections();
		float reticleScale = human.Reticle.image.rectTransform.localScale.x;
		for (int i=0; i<this.DirectionIndicators.Count; i++)
		{
			Image img = this.DirectionIndicators[i];
			if (i < moves.Count)
			{
				img.enabled = true;
				//img.tras
				img.transform.SetParent(human.Reticle.transform);
				img.transform.localPosition = reticleScale * 40f * moves[i];
			}
			else 
			{
				img.enabled = false;
			}
		}
	}
}

}

