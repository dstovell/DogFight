using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DogFight
{

public class ArenaPanel : MonoBehaviour
{

	public float WarpInDelay = 0f;
	public float WarpInFrame = -1;
	public GameObject WarpIn;

	public float LaunchDelay = 0f;
	public float LaunchAllFrame = -1;

	public Camera panelCam;
	public Opertoon.Panoply.Panel panel;

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
		//Debug.LogError("DisableAll count=" + ArenaPanel.panels.Count);
		for (int i=0; i<ArenaPanel.panels.Count; i++)
		{
			ArenaPanel.panels[i].DisablePanel();
		}
	}

	void Awake()
	{
		Opertoon.Panoply.PanoplyEventManager.OnTargetStepChanged += OnTargetStepChanged;

		ArenaPanel.panels.Add(this);
		if (this.panelCam == null)
		{
			this.panelCam = this.gameObject.GetComponent<Camera>();
		}
		if (this.panel == null)
		{
			this.panel = this.gameObject.GetComponent<Opertoon.Panoply.Panel>();
		}
	}

	void Start()
	{
		StartCoroutine(StepInitialFrame());
	}

	void OnDestroy()
	{
		Opertoon.Panoply.PanoplyEventManager.OnTargetStepChanged -= OnTargetStepChanged;

		ArenaPanel.panels.Remove(this);
	}

	//They don't give us this callback, so lets make our own!
	private IEnumerator StepInitialFrame()
	{
		yield return new WaitForSeconds(0.2f);

		this.OnTargetStepChanged(0, 1);
	}

	private void OnTargetStepChanged(int oldStep, int newStep)
	{
		//Debug.LogError("OnTargetStepChanged oldStep=" + oldStep + " newStep=" + newStep);
		HandleWarpIn(oldStep, newStep);
		HandleLaunch(oldStep, newStep);
	}

	void HandleWarpIn(int oldStep, int newStep)
	{
		if ((this.WarpIn != null) && (this.WarpInFrame == newStep))
		{
			Opertoon.Panoply.CameraState camState = (this.panel.CurrentCameraState() != null) ? this.panel.CurrentCameraState() : this.panel.NextCameraState();

			Vector3 lookAt = camState.lookAt;
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

				//Debug.LogError("WarpIn pos=" + obj.transform.position);

				ship.Warp(obj.transform);
			}

			this.WarpIn = null;
		}
	}

	void HandleLaunch(int oldStep, int newStep)
	{
		if (this.LaunchAllFrame >= 0)
		{
			//Debug.LogError("this.LaunchAllFrame=" + this.LaunchAllFrame + " newStep=" + newStep);
			if (this.LaunchAllFrame == newStep)
			{
				//Debug.LogError("HandleLaunch LaunchDelay=" + this.LaunchDelay );
				StartCoroutine(LaunchAll(this.LaunchDelay));

				this.LaunchAllFrame = -1;
			}
		}
	}

	private IEnumerator LaunchAll(float secs)
    {
		//Debug.LogError("LaunchAll secs=" + secs );
		if (secs != 0) yield return new WaitForSeconds(secs);
		ShipController.LaunchAll();
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