using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DogFight
{
	public class StaticObstacle : MonoBehaviour 
	{
		static private List<StaticObstacle> Obstacles = new List<StaticObstacle>();

		static public bool AnyContains(Vector3 point)
		{
			for (int i=0; i<StaticObstacle.Obstacles.Count; i++)
			{
				if (StaticObstacle.Obstacles[i].Contains(point))
				{
					return true;
				}
			}
			return false;
		}

		public bool Contains(Vector3 point)
		{
			Collider col = this.gameObject.GetComponent<Collider>();
			if ((col != null) && col.bounds.Contains(point))
			{
				return true;
			}

			Collider [] cols = this.gameObject.GetComponentsInChildren<Collider>();
			for (int i=0; i<cols.Length; i++)
			{
				if (cols[i].bounds.Contains(point))
				{
					return true;
				}
			}

			return false;
		}

		void Awake()
		{
			StaticObstacle.Obstacles.Add(this);
		}

		void OnDestroy()
		{
			StaticObstacle.Obstacles.Remove(this);
		}

		// Use this for initialization
		void Start () 
		{
		
		}
		
		// Update is called once per frame
		void Update () 
		{
		
		}
	}
}