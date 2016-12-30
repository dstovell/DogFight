using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DogFight
{

public class ArenaPanel : MonoBehaviour
{
	public GameObject WarpIn;

	public Camera panelCam;

	private static List<ArenaPanel> panels = new List<ArenaPanel>();

	static public void EnableAll()
	{
		for (int i=0; i<ArenaPanel.panels.Count; i++)
		{
			ArenaPanel.panels[i].EnablePanel();
		}
	}
	
	static public void DisableAll()
	{
		Debug.LogError("DisableAll count=" + ArenaPanel.panels.Count);
		for (int i=0; i<ArenaPanel.panels.Count; i++)
		{
			ArenaPanel.panels[i].DisablePanel();
		}
	}

	void Awake()
	{
		ArenaPanel.panels.Add(this);
		if (this.panelCam == null)
		{
			this.panelCam = this.gameObject.GetComponent<Camera>();
		}
	}

	void OnDestroy()
	{
		ArenaPanel.panels.Remove(this);
	}

	void HandleWarpIn()
	{
		if (this.WarpIn != null)
		{
			Opertoon.Panoply.Panel p = this.gameObject.GetComponent<Opertoon.Panoply.Panel>();
			Vector3 lookAt = p.CurrentCameraState().lookAt;
			GameObject obj = GameObject.Instantiate(this.WarpIn, lookAt, Quaternion.identity) as GameObject;
			if (obj != null)
			{
				ShipController ship = obj.GetComponent<ShipController>();
				if (ship == null)
				{
					GameObject.Destroy(obj);
					Debug.LogError("WarpIn no Ship");
					return;
				}

				Debug.LogError("WarpIn pos=" + obj.transform.position);

				ship.Warp(obj.transform);
			}

			this.WarpIn = null;
		}
	}

	void Update()
	{
		if (this.panelCam.enabled)
		{
			HandleWarpIn();
		}
	}

	public void EnablePanel()
	{
		this.gameObject.SetActive(true);
	}

	public void DisablePanel()
	{
		this.gameObject.SetActive(false);
	}
}

}