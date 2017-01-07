using UnityEngine;
using System.Collections;

namespace DSTools
{

public abstract class ConditionalAction : MonoBehaviour
{
	public enum TriggerType
	{
		Update,
		CollisionOrTrigger,
		Collision,
		Trigger
	}
	public TriggerType Trigger;
	public int MaxTriggerCount = 0;
	private int TriggerCount = 0;
	protected bool Running = true;

	public enum ConditionJoinType
	{
		And,
		Or,
		XOr
	}
	public ConditionJoinType JoinType;
	public Condition [] Conditions;

	public float DelaySeconds = 0f;

	protected abstract bool OnConditionsMet(GameObject triggerer);

	private bool AreConditionsMet()
	{
		bool allTrue = true;
		bool atLeastOneTrue = false;
		bool exactlyOneTrue = false; 

		for (int i=0; i<this.Conditions.Length; i++)
		{
			Condition c = this.Conditions[i];
			if (c != null)
			{
				if (c.IsConditionMet())
				{
					if (exactlyOneTrue)
					{
						exactlyOneTrue = false;
					}
					else 
					{
						atLeastOneTrue = true;
						exactlyOneTrue = true;
					}
				}
				else
				{
					allTrue = false;
				}
			}
		}

		switch (this.JoinType)
		{
			case ConditionJoinType.And:	return allTrue;
			case ConditionJoinType.Or: 	return atLeastOneTrue;
			case ConditionJoinType.XOr:	return exactlyOneTrue;
		}

		return false;
	}

	private void CheckForTrigger(GameObject trigerer = null)
	{
		if ( this.Running && ((this.MaxTriggerCount == 0) || (this.TriggerCount < this.MaxTriggerCount)) )
		{
			if (this.AreConditionsMet())
			{
				StartCoroutine(this.CallOnConditionsMet(trigerer));
				this.TriggerCount++;
			}
		}
	}

	private IEnumerator CallOnConditionsMet(GameObject trigerer)
	{
		if (this.DelaySeconds > 0f) yield return new WaitForSeconds(this.DelaySeconds);

		bool success = this.OnConditionsMet(trigerer);

		if (!success)
		{
			this.TriggerCount--;
		}
	}

	void Update()
	{
		if (this.Trigger == TriggerType.Update)
		{
			this.CheckForTrigger();
		}
	}

	void OnTriggerEnter(Collider other)
    {
		if ((this.Trigger == TriggerType.Trigger) || (this.Trigger == TriggerType.CollisionOrTrigger))
		{
			this.CheckForTrigger(other.gameObject);
		}
    }

    void OnCollisionEnter(Collision other)
    {
		if ((this.Trigger == TriggerType.Collision) || (this.Trigger == TriggerType.CollisionOrTrigger))
		{
			this.CheckForTrigger(other.gameObject);
		}
    }
}

}
