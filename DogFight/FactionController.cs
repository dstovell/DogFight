using UnityEngine;
using System.Collections;

namespace DogFight
{

public enum FactionType
{
	Alliance,
	Augments,
	Pirate
}

public class FactionController : MonoBehaviour
{
	public static FactionController Instance;

	void Awake()
	{
		FactionController.Instance = this;
	}
	
	public bool IsHostile(FactionType t1, FactionType t2)
	{
		if (t1 == t2)
		{
			return false;
		}

		return true;
	}

	public bool IsAllied(FactionType t1, FactionType t2)
	{
		if (t1 == t2)
		{
			return true;
		}

		return !this.IsHostile(t1, t2);
	}
}

}