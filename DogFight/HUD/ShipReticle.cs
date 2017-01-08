using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShipReticle : MonoBehaviour
{
	public Image image;
	public GameObject obj;

	public GameObject lookAt;

	void Awake()
	{
		if ((this.image == null) && (this.obj == null))
		{
			this.obj = this.gameObject;
		}
	}

	public void SetPosition(Vector3 pos)
	{
		if (image != null)
		{
			image.rectTransform.localPosition = pos;
		}

		if (obj != null)
		{
			obj.transform.localPosition = pos;
		}
	}

	public void SetScale(Vector3 scaleV)
	{
		if (image != null)
		{
			image.rectTransform.localScale = scaleV;
		}

		if (obj != null)
		{
			obj.transform.localScale = scaleV;
		}
	}

	void Update()
	{
		if (this.lookAt == null)
		{
			return;
		}

		if (image != null)
		{
			Vector3 pos = Camera.main.WorldToScreenPoint( lookAt.transform.position );
			pos.z = 0f;
			this.transform.position = pos;
			//image.enabled = true;
		}

//		if (obj != null)
//		{
//			this.transform.position = target.transform.position;
//			Vector3 pos = Camera.main.WorldToScreenPoint( target.transform.position );
//			pos.z = 0f;
//			this.transform.position = pos;
//		}
	}
}

