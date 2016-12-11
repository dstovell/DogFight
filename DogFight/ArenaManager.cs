using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DogFight
{

public class ArenaManager : MonoBehaviour
{
	public static ArenaManager Instance = null;

	public GameObject PathHighlight;

	public enum State
	{
		Init,
		Intro,
		Gameplay,
		Outro
	}
	public State CurrentState = State.Init;

	public void HighlightPath(List<Vector3> path)
	{
		if (this.PathHighlight != null)	
		{
			GameObject highlight = GameObject.Instantiate(this.PathHighlight, path[0], Quaternion.identity) as GameObject;
			ShipLeader leader = highlight.GetComponent<ShipLeader>();
			if (leader == null)
			{
				leader = highlight.AddComponent<ShipLeader>();
			}

			leader.SetSpeed(500);
			leader.MovePath(path, false);
		}
	}

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
