using UnityEngine;
using System.Collections;

namespace DogFight
{

public class Arena : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		GenerateArena();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public float ArenaRadius = 50.0f;
	public float ArenaLength = 100.0f;

	public int ArenaLayers = 2;
	public int ArenaArcSegments = 10;
	public int ArenaSegments = 50;

	public void GenerateSegment(Vector3 centerPos, float radius)
	{
		float arcSize = (2*Mathf.PI)/this.ArenaArcSegments;
		for (int i=0; i<this.ArenaArcSegments; i++)
		{
			float angle = (float)i * arcSize;
			float x = radius * Mathf.Cos(angle);
			float y = radius * Mathf.Sin(angle);
			Vector3 pos = centerPos + new Vector3(x, y);
			GameObject obj = new GameObject("NavPoint");
			obj.tag = "NavPoint";
			obj.transform.position = pos;
			obj.transform.SetParent(this.transform);
		}
	}

	public void GenerateArena()
	{
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
	}
}

}
