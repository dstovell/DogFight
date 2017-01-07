using UnityEngine;
using System.Collections;

namespace DogFight
{

public class IsPanoplyFrameCondition : DSTools.Condition
{
	public int frame = 0;
	private int currentFrame = 0;

	public override bool IsConditionMet()
	{
		return (this.currentFrame == this.frame);
	}

	void Awake()
	{
		Opertoon.Panoply.PanoplyEventManager.OnTargetStepChanged += OnTargetStepChanged;
	}

	void OnDestroy()
	{
		Opertoon.Panoply.PanoplyEventManager.OnTargetStepChanged -= OnTargetStepChanged;
	}

	private void OnTargetStepChanged(int oldStep, int newStep)
	{
		this.currentFrame = newStep;
	}
}

}
