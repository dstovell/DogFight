using UnityEngine;
using System.Collections;

namespace DogFight
{

public class ArenaManager : MonoBehaviour
{
	public static ArenaManager Instance = null;

	public enum State
	{
		Init,
		Intro,
		Gameplay,
		Outro
	}
	public State CurrentState = State.Init;

	void Awake()
	{
		if (ArenaManager.Instance == null)
		{
			ArenaManager.Instance = this;
		}
	}

	void OnDestroy()
	{
		if (ArenaManager.Instance == this)
		{
			ArenaManager.Instance = null;
		}
	}

	void Start()
	{
		this.UpdateState();
	}

	void Update ()
	{
		this.UpdateState();
	}

	public void UpdateState()
	{
		if (this.CurrentState == State.Init)
		{
			Arena.Instance.GenerateArena();

			for (int i=0; i<Team.Teams.Count; i++)
			{
				Team.Teams[i].SpawnTeamMembers();
			}

			this.CurrentState = State.Intro;
		}
		else if (this.CurrentState == State.Intro)
		{
			this.CurrentState = State.Gameplay;
		}
		else if (this.CurrentState == State.Gameplay)
		{
		}
		else if (this.CurrentState == State.Outro)
		{
		}
	}
}

}
