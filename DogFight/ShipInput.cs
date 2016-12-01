using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TouchScript;
using TouchScript.Gestures;

namespace DogFight
{

public class ShipInput : DSTools.MessengerListener
{
	public Camera cam;

	void Awake()
	{
		if (this.cam == null)
		{
			this.cam = Camera.main;
		}
	}

	void Start()
	{
		this.InitMessenger("ShipInput");
	}

	void Update() 
	{
	}

	private void OnEnable()
    {
		FlickGesture [] flicks = this.cam.gameObject.GetComponents<FlickGesture>();
		for (int i=0; i<flicks.Length; i++)
		{
			Debug.LogError("ShipInput flick=" + flicks[i].name);
			flicks[i].Flicked += this.FlickedHandler;
		}

		TapGesture [] taps = this.cam.gameObject.GetComponents<TapGesture>();
		for (int i=0; i<taps.Length; i++)
		{
			Debug.LogError("ShipInput tap=" + taps[i].name);
			taps[i].Tapped += this.TapHandler;
		}


		ScreenTransformGesture [] trans = this.cam.gameObject.GetComponents<ScreenTransformGesture>();
		for (int i=0; i<trans.Length; i++)
		{
			Debug.LogError("ShipInput ScreenTransformGesture=" + trans[i].name);
			trans[i].Transformed += this.TransformedHandler;
			trans[i].TransformCompleted += this.TransformCompletedHandler;
		}
    }

    private void OnDisable()
    {
		if (this.cam == null)
		{
			return;
		}

		FlickGesture [] flicks = this.cam.gameObject.GetComponents<FlickGesture>();
		for (int i=0; i<flicks.Length; i++)
		{
			flicks[i].Flicked -= this.FlickedHandler;
		}

		TapGesture [] taps = this.cam.gameObject.GetComponents<TapGesture>();
		for (int i=0; i<taps.Length; i++)
		{
			taps[i].Tapped -= this.TapHandler;
		}

		TransformGesture [] trans = this.cam.gameObject.GetComponents<TransformGesture>();
		for (int i=0; i<trans.Length; i++)
		{
			trans[i].Transformed -= this.TransformedHandler;
			trans[i].TransformCompleted -= this.TransformCompletedHandler;
		}
    }

	private void FlickedHandler(object sender, EventArgs e)
	{
		Debug.LogError("FlickedHandler");
		FlickGesture gesture = sender as FlickGesture;
		if (gesture != null)
		{
			Debug.LogError("FlickedHandler got FlickGesture Direction=" + gesture.Direction.ToString() + " ScreenFlickVector=" + gesture.ScreenFlickVector.x + "," + gesture.ScreenFlickVector.y);
			Vector2 flickVector = gesture.ScreenFlickVector;
			this.SendMessengerMsg("flick", flickVector);
		}
	}

	private void TapHandler(object sender, EventArgs e)
	{
		Debug.LogError("TapHandler");
		TapGesture gesture = sender as TapGesture;
		if (gesture != null)
		{
			Vector2 tapPoint = gesture.ScreenPosition;

			this.SendMessengerMsg("tap", tapPoint);

			//RaycastHit hitPoint = new RaycastHit();
			//Ray ray = Camera.main.ScreenPointToRay(tapPoint);
			//float rayDistance = 100f;
			//string [] maskStrings = new string[2]{"Cube","CubeRow"};
			//LayerMask mask = LayerMask.GetMask(maskStrings);

			//if (Physics.Raycast(ray, out hitPoint, rayDistance, mask))
	        //{
	        //	GameObject obj = hitPoint.collider.gameObject;
			//}
		}
	}

	private void TransformedHandler(object sender, EventArgs e)
	{
		//ScreenTransformGesture
		ScreenTransformGesture gesture = sender as ScreenTransformGesture;
		if (gesture != null)
		{
		}
	}

	private void TransformCompletedHandler(object sender, EventArgs e)
	{
	}

	public override void OnMessage(string id, object obj1, object obj2)
	{
		switch(id)
		{
		default:
			break;
		}
	}
}

}