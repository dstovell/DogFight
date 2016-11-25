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
	}

	// Use this for initialization
	void Start() 
	{
		GenerateArena();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public float ArenaRadius = 50.0f;
	public float ArenaLength = 100.0f;
	public float ArenaCapLength = 20.0f;

	public int ArenaLayers = 2;
	public int ArenaArcSegments = 6;
	public int ArenaSegments = 50;

	public GameObject EndA;
	public GameObject EndB;

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

	private void GenerateSegment(Vector3 centerPos, float radius)
	{
		float arcSize = (2*Mathf.PI)/this.ArenaArcSegments;
		for (int i=0; i<this.ArenaArcSegments; i++)
		{
			float angle = (float)i * arcSize;
			float x = radius * Mathf.Cos(angle);
			float y = radius * Mathf.Sin(angle);
			Vector3 pos = centerPos + new Vector3(x, y);
			this.AddNavPoint(pos, this.NavPoints.transform);
		}

		this.AddNavPoint(centerPos, this.NavPoints.transform);
	}

	public void GenerateArena()
	{
		this.NavPoints = new GameObject("NavPoints");
		this.NavPoints.transform.SetParent(this.transform);
		float segmentLength = this.ArenaLength / (float)this.ArenaSegments;
		float layerMultiple = this.ArenaRadius / (float)this.ArenaLayers;
		for (int i=0; i<this.ArenaLayers; i++)
		{
			float radius = this.ArenaRadius - (float)i * layerMultiple;
			for (int j=0; j<this.ArenaSegments; j++)
			{
				Vector3 pos = new Vector3(0.0f, 0.0f, (float)j * segmentLength);
				this.GenerateSegment(pos, radius);
			}
		}

		this.EndA = this.AddNavPoint(new Vector3(0.0f, 0.0f, -1.0f*this.ArenaCapLength), this.transform);
		this.EndA.name = "EndA";
		this.EndB = this.AddNavPoint(new Vector3(0.0f, 0.0f, this.ArenaLength - segmentLength + this.ArenaCapLength), this.transform);
		this.EndB.name = "EndB";

		AstarPath.active.astarData.pointGraph.limits = new Vector3(layerMultiple, layerMultiple, 1.5f*segmentLength);
		AstarPath.active.astarData.pointGraph.maxDistance = 1.5f*segmentLength;
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
