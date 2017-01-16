using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TouchScript;
using TouchScript.Gestures;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DogFight
{

public class ShipInput : DSTools.MessengerListener
{
	public Camera cam;

	public UltimateJoystick Joystick;
	public float JoyStickMultiplier = 300f;

	public UltimateJoystick Throttle;
	float ThrottleDeadZone = 0.2f;

	public UltimateButton [] Buttons;

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

		if (this.Buttons.Length > 0)
		{
			this.Buttons[0].onButtonDown.AddListener(this.OnButtonDown);
			this.Buttons[0].onButtonUp.AddListener(this.OnButtonUp);
		}
	}

	void Update() 
	{
		if (this.Human != null)
		{
			if (this.IsJoystick())
			{
				Vector2 pos = this.Joystick.GetPosition();
				Vector2 deltaPos = (this.JoyStickMultiplier * Time.deltaTime) * pos;
				//Debug.Log("pos=" + pos.ToString() + " deltaPos=" + deltaPos);
				this.Human.HandleTransform(deltaPos);
			}

			if (this.Throttle != null)
			{
				bool isThrottleTouched = this.Throttle.GetJoystickState();
				float throttleAmount = this.Throttle.GetVerticalAxis();
				//Debug.Log("isThrottleTouched=" + isThrottleTouched + " throttleAmount=" + throttleAmount);
				if (isThrottleTouched)
				{
					if (Mathf.Abs(throttleAmount) >= this.ThrottleDeadZone)
					{
						this.StopFiring();
						this.Human.HandleThrottle(throttleAmount);
					}
					else
					{
						this.StartFiring();
						this.Human.HandleThrottle(0f);
					}
				}
				else 
				{
					this.StopFiring();
					this.Human.HandleThrottle(0f);
				}
			}
		}
	}

	private void StartFiring()
	{
		this.Human.StartFiring();
	}

	private void StopFiring()
	{
		this.Human.StopFiring();
	}

	public void OnButtonDown()
	{
		this.StartFiring();
	}

	public void OnButtonUp()
	{
		this.StopFiring();
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

		//Debug.LogError("TransformedHandler");
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