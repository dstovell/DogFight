using UnityEngine;
using System.Collections;

namespace DogFight
{

public class DisablePanoplyConditionalAction : DSTools.ConditionalAction
{
	public GameObject PanoplyObj;

	public Opertoon.Panoply.Panel [] StoryPanels;

	public Opertoon.Panoply.Panel GameplayPanel;

	protected override bool OnConditionsMet(GameObject triggerer)
	{
		if (this.PanoplyObj != null)
		{
			this.PanoplyObj.SetActive(false);
		}

		for (int i=0; i<this.StoryPanels.Length; i++)
		{
			if (this.StoryPanels[i] != null)
			{
				this.StoryPanels[i].gameObject.SetActive(false);
			}
		}

		if (this.GameplayPanel != null)
		{
			this.GameplayPanel.enabled = false;
		}

		return true;
	}
}

}


