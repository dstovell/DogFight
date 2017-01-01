using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DogFight
{

public class ArenaPanel : MonoBehaviour
{
	public GameObject ActionTarget;

	public float WarpInDelay = 0f;
	public float WarpInFrame = -1;

	public float HangerOpenDelay = 0f;
	public float HangerOpenFrame = -1;

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
		HandleOpenHanger(oldStep, newStep);
		HandleLaunch(oldStep, newStep);
	}

	void HandleWarpIn(int oldStep, int newStep)
	{
		if ((this.ActionTarget != null) && (this.WarpInFrame == newStep))
		{
			StartCoroutine(WarpIn(this.WarpInDelay));
		}
	}

	private IEnumerator WarpIn(float secs)
    {
		//Debug.LogError("WarpIn secs=" + secs );
		if (secs != 0) yield return new WaitForSeconds(secs);

		Opertoon.Panoply.CameraState camState = (this.panel.CurrentCameraState() != null) ? this.panel.CurrentCameraState() : this.panel.NextCameraState();

		this.ActionTarget.transform.SetParent(null);

		Vector3 lookAt = camState.lookAt;
		this.ActionTarget.transform.position = lookAt;
		//this.ActionTarget.transform.rotation = Quaternion.identity;				

		ShipController ship = this.ActionTarget.GetComponent<ShipController>();
		if (ship != null)
		{
			ship.Warp(this.ActionTarget.transform);
		}

		this.ActionTarget = null;
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

	void HandleOpenHanger(int oldStep, int newStep)
	{
		if (this.HangerOpenFrame >= 0)
		{
			if (this.HangerOpenFrame == newStep)
			{
				StartCoroutine(OpenAllHangers(this.HangerOpenDelay));

				this.HangerOpenFrame = -1;
			}
		}
	}

	private IEnumerator OpenAllHangers(float secs)
    {
		//Debug.LogError("LaunchAll secs=" + secs );
		if (secs != 0) yield return new WaitForSeconds(secs);
		ShipController.OpenAllHangers();
    }

	public void EnablePanel()
	{
		if (this.gameObject.tag == "MainCamera")
		{
			return;
		}

		this.gameObject.SetActive(true);
	}

	public void DisablePanel()
	{
		if (this.gameObject.tag == "MainCamera")
		{
			return;
		}

		this.gameObject.SetActive(false);
	}
}

}