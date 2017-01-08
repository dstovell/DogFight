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

	public UltimateJoystick Joystick;
	public float JoyStickMultiplier = 300f;

	private Combatant Human;

	void Awake()
	{
		if (this.cam == null)
		{
			this.cam = Camera.main;
			if (this.cam == null)
			{
				this.cam = this.gameObject.GetComponent<Camera>();
			}
		}
	}

	void Start()
	{
		this.InitMessenger("ShipInput");
		this.Human = ShipController.GetHuman();
	}

	void Update() 
	{
		if (this.IsJoystick() && (this.Human != null))
		{
			Vector2 pos = this.Joystick.GetPosition();
			Vector2 deltaPos = (this.JoyStickMultiplier * Time.deltaTime) * pos;
			//Debug.Log("pos=" + pos.ToString() + " deltaPos=" + deltaPos);
			this.Human.HandleTransform(deltaPos);
		}
	}

	public bool IsJoystick()
	{
		return ((this.Joystick != null) && this.Joystick.gameObject.activeInHierarchy);
	}

	private void OnEnable()
    {
		FlickGesture [] flicks = this.cam.gameObject.GetComponents<FlickGesture>();
		for (int i=0; i<flicks.Length; i++)
		{
			//Debug.LogError("ShipInput flick=" + flicks[i].name);
			flicks[i].Flicked += this.FlickedHandler;
		}

		TapGesture [] taps = this.cam.gameObject.GetComponents<TapGesture>();
		for (int i=0; i<taps.Length; i++)
		{
			//Debug.LogError("ShipInput tap=" + taps[i].name);
			taps[i].Tapped += this.TapHandler;
		}


		ScreenTransformGesture [] trans = this.cam.gameObject.GetComponents<ScreenTransformGesture>();
		for (int i=0; i<trans.Length; i++)
		{
			//Debug.LogError("ShipInput ScreenTransformGesture=" + trans[i].name);
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
		if (this.IsJoystick())
		{
			return;
		}

		//Debug.LogError("FlickedHandler");
		FlickGesture gesture = sender as FlickGesture;
		if (gesture != null)
		{
			//Debug.LogError("FlickedHandler got FlickGesture Direction=" + gesture.Direction.ToString() + " ScreenFlickVector=" + gesture.ScreenFlickVector.x + "," + gesture.ScreenFlickVector.y);
			Vector2 flickVector = gesture.ScreenFlickVector;
			this.Human.HandleFlick(flickVector);
			//this.SendMessengerMsg("flick", flickVector);
		}
	}

	private void TapHandler(object sender, EventArgs e)
	{
		if (this.IsJoystick())
		{
			return;
		}

		Debug.LogError("TapHandler");
		TapGesture gesture = sender as TapGesture;
		if (gesture != null)
		{
			Vector2 tapPoint = gesture.ScreenPosition;
			this.Human.HandleTap(tapPoint);

			//this.SendMessengerMsg("tap", tapPoint);
		}
	}

	private void TransformedHandler(object sender, EventArgs e)
	{
		if (this.IsJoystick())
		{
			return;
		}

		Debug.LogError("TransformedHandler");
		//ScreenTransformGesture
		ScreenTransformGesture gesture = sender as ScreenTransformGesture;
		if (gesture != null)
		{
			Vector2 deltaPos = gesture.ScreenPosition - gesture.PreviousScreenPosition;
			this.Human.HandleTransform(deltaPos);
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