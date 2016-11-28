using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpriteObjectTracker : MonoBehaviour {

	public Image image;

	public GameObject target;

	public GameObject viewPosition;
	public float minRange = 0f;
	public float fullSizeRange = 0f;

	void Start () 
	{
		UpdatePosition();
	}
	
	void Update () 
	{
		UpdatePosition();
	}

	public void SetScale(float scale)
	{
		if (image == null)
		{
			return;
		}

		image.rectTransform.localScale = new Vector3(scale, scale, scale);
	}

	public void SetScale(Vector3 scaleV)
	{
		if (scaleV == null)
		{
			return;
		}

		image.rectTransform.localScale = scaleV;
	}

	void UpdatePosition()
	{
		if (image == null)
		{
			return;
		}

		if (target == null)
		{
			image.enabled = false;
			return;
		}

		if (minRange > 0f)
		{
			Vector3 viewPos = (viewPosition != null) ? viewPosition.transform.position : Vector3.zero;
			float dist = Vector3.Distance(viewPos, target.transform.position);
			if (dist > minRange)
			{
				image.enabled = false;
				return;
			}

			float deltaZ = viewPos.z - target.transform.position.z;
			if ( ((deltaZ > 0) && (viewPosition.transform.forward.z > 0)) || ((deltaZ < 0) && (viewPosition.transform.forward.z < 0)) )
			{
				image.enabled = false;
				return;
			}

			if (fullSizeRange > 0)
			{
				float scale = 1f;
				if (dist > fullSizeRange)
				{
					float t = dist - fullSizeRange;
					float interval = minRange - fullSizeRange;
					scale = 1f - t/interval;
				}
				this.SetScale(scale);
			}
		}

		Vector3 pos = Camera.main.WorldToScreenPoint( target.transform.position );
		pos.z = 0f;
		this.transform.position = pos;
		image.enabled = true;
	}
}
