using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoadTimer : MonoBehaviour 
{
	public float SecondsBeforeLoad = 5;
	public int SceneIndex = 1;

	private float ElapsedTime = 0f;
	private bool done = false;
	
	void Update() 
	{
		if (done)
		{
			return;
		}

		this.ElapsedTime += Time.deltaTime;

		if (this.ElapsedTime > this.SecondsBeforeLoad)
		{
			done = true;
			SceneManager.LoadScene(this.SceneIndex);
		}
	}
}
