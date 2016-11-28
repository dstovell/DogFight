using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

namespace DogFight
{

public class Arena : MonoBehaviour {

	static public Arena Instance;

	void Awake()
	{
		Arena.Instance = this;
		GenerateArena();
	}

	void Start() 
	{
	}

	void Update() 
	{
	}

	public float ArenaHeight = 100f;
	public float ArenaWidth = 100f;
	public float ArenaLength = 500f;
	public float ArenaCapLength = 20f;

	public float ArenaSegmentSpacing = 20f;
	public float ArenaSegmentLength = 40f;

	public GameObject EndA;
	public GameObject EndB;

	public float AsteroidSpawnCount = 20;
	public GameObject [] Asteroids;

	private GameObject NavPoints;

	private GameObject AddNavPoint(Vector3 pos, Transform parent)
	{
		if (StaticObstacle.AnyContains(pos))
		{
			return null;
		}

		GameObject obj = new GameObject("NavPoint");
		obj.tag = "NavPoint";
		obj.transform.position = pos;
		obj.transform.SetParent(parent);
		return obj;
	}

	private void GenerateSegment(Vector3 centerPos, List<Vector3> navPointPositions)
	{
		for (float i=0; i<this.ArenaWidth; i+=this.ArenaSegmentSpacing)
		{
			for (float j=0; j<this.ArenaHeight; j+=this.ArenaSegmentSpacing)
			{
				Vector3 pos = centerPos + new Vector3(i, j, 0f);
				navPointPositions.Add(pos);
			}
		}
	}


	private void SpawnRandomAsteroid(List<Vector3> possiblePositions, GameObject parent = null)
	{
		if ((possiblePositions.Count == 0) || (this.Asteroids.Length == 0))
		{
			return;
		}

		int randomPosIndex = Random.Range(0, possiblePositions.Count-1);
		int randomAsteroid = Random.Range(0, this.Asteroids.Length);

		Quaternion randomRotation = Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f));
		float randomScale = Random.Range(0.3f, 1.0f);
		//Squish upper size 1-(x-1)^2
		GameObject asteroid = GameObject.Instantiate(this.Asteroids[randomAsteroid], possiblePositions[randomPosIndex], randomRotation) as GameObject;
		asteroid.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
		if (parent != null)
		{
			asteroid.transform.SetParent(parent.transform);	
		}
	}

	public void GenerateArena()
	{
		List<Vector3> navPointPositions = new List<Vector3>();

		float centerX = -1f * this.ArenaWidth/2f;
		float centerY = -1f * this.ArenaHeight/2f;
		this.NavPoints = new GameObject("NavPoints");
		this.NavPoints.transform.SetParent(this.transform);
		for (float i=0; i<this.ArenaLength; i+=this.ArenaSegmentLength)
		{
			Vector3 pos = new Vector3(centerX, centerY, i);
			this.GenerateSegment(pos, navPointPositions);
		}

		this.EndA = this.AddNavPoint(new Vector3(0f, 0f, -1.0f*this.ArenaCapLength), this.transform);
		this.EndA.name = "EndA";
		this.EndB = this.AddNavPoint(new Vector3(0f, 0f, this.ArenaLength - this.ArenaSegmentLength + this.ArenaCapLength), this.transform);
		this.EndB.name = "EndB";

		GameObject asteroidContainer = new GameObject("Asteroids");
		asteroidContainer.transform.SetParent(this.transform);
		for (int a=0; a<this.AsteroidSpawnCount; a++)
		{
			this.SpawnRandomAsteroid(navPointPositions, asteroidContainer);
		}

		for (int n=0; n<navPointPositions.Count; n++)
		{
			this.AddNavPoint(navPointPositions[n], this.NavPoints.transform);
		}

		float xyMaxDist = 1.5f*this.ArenaSegmentSpacing;
		float zMaxDist = 1.5f*this.ArenaSegmentLength;
		AstarPath.active.astarData.pointGraph.limits = new Vector3(xyMaxDist, xyMaxDist, zMaxDist);
		AstarPath.active.astarData.pointGraph.maxDistance = zMaxDist;
		AstarPath.active.Scan();

		AstarPath.RegisterSafeUpdate (() => {
			PointGraph graph = AstarPath.active.astarData.pointGraph;
			for (int i=0; i<graph.nodes.Length; i++)
			{
				PointNode node = graph.nodes[i];
				if (node != null)
				{
					List<GraphNode> connsToRemove = new List<GraphNode>();
					for (int j=0; j<node.connections.Length; j++)
					{
						GraphNode n = node.connections[j];
						if (n.position.z == node.position.z)
						{
							connsToRemove.Add(n);
						}
					}

					for (int j=0; j<connsToRemove.Count; j++)
					{
						node.RemoveConnection(connsToRemove[j]);
					}
				}
			}
		});
	}
}

}
