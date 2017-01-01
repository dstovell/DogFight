using UnityEngine;
using System.Collections;
using DSTools;

namespace DogFight
{

public class CameraManager : MessengerListener
{
	public static CameraManager Instance = null;

	public Camera Cam;
	public KGFOrbitCamSettings FollowCam;
	public Opertoon.Panoply.Panel panel;

	public Camera GetCamera()
	{
		return (this.Cam != null) ? this.Cam : Camera.main;
	}

	public KGFOrbitCam GetOrbitCamera()
	{
		return this.GetCamera().GetComponent<KGFOrbitCam>();
	}

	public void EnableFollowCam(GameObject target, GameObject lookAt)
	{
		if (this.FollowCam != null)
		{
			this.panel.enabled = false;
			GetCamera().enabled = true;
			KGFOrbitCam orbitCam = this.GetOrbitCamera();
			orbitCam.gameObject.tag = "MainCamera";
			this.FollowCam.itsOrbitCam.itsOrbitCam = orbitCam;
			this.FollowCam.itsTarget.itsTarget = target;

			if (lookAt != null)
			{
				this.FollowCam.itsLookat.itsEnable = true;
				this.FollowCam.itsLookat.itsLookatTarget = lookAt;
				this.FollowCam.itsLookat.itsUpVectorSource = target;
			}

			this.FollowCam.Apply();
		}
	}

	void Awake()
	{
		if (CameraManager.Instance == null)
		{
			CameraManager.Instance = this;
		}
	}

	public override void OnMessage(string id, object obj1, object obj2)
	{
	}
}

}
