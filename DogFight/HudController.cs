using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace DogFight
{

public class HudController : MonoBehaviour
{
	static public HudController Instance;

	public GameObject ReticleObj;
	public GameObject TargetPrefab;
	public GameObject JoyStick;
	public GameObject Throttle;

	public GameObject [] Buttons;

	public List<SpriteObjectTracker> Targets;

	public void CreateHud(ShipController humanShip, GameObject lookAt)
	{
		this.ReticleObj.SetActive(true);
		ShipReticle reticle = this.ReticleObj.GetComponent<ShipReticle>();
		reticle.SetPosition(Vector3.zero);
		reticle.lookAt = lookAt;
		humanShip.Reticle = reticle;

		if (this.JoyStick != null)
		{
			this.JoyStick.SetActive(true);
		}

		if (this.Throttle != null)
		{
			this.Throttle.SetActive(true);
		}

		for (var i=0; i<this.Buttons.Length; i++)
		{
			this.Buttons[i].SetActive(true);
		}
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
	}
}

}

