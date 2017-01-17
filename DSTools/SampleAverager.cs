using UnityEngine;
using System.Collections;

public class FloatAverager
{
	float [] values;
	//int frontIndex = 0;
	int nextIndex = 0;
	int valuesWritten = 0;
	
	public FloatAverager(int numFloats)
	{
		this.values = new float[numFloats];
	}

	public void AddValue(float v)
	{
		this.values[nextIndex] = v;
		this.nextIndex++;
		if (this.nextIndex >= this.values.Length)
		{
			this.nextIndex = 0;
		}
		this.valuesWritten = Mathf.Min(valuesWritten+1, this.values.Length);
	}

	public float GetLinearAverage()
	{
		float total = 0;
		for (int i=0; i<this.valuesWritten; i++)
		{
			total += this.values[i];
		}
		return total / (float)this.valuesWritten;
	}
}

