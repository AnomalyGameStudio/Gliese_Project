using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour 
{
	float sizeY;
	bool open = false;

	void Start()
	{
		BoxCollider collider = GetComponent<BoxCollider>();
		sizeY = collider.bounds.max.y;
	}

	public void Open()
	{
		if(!open)
		{
			Vector3 newPos = new Vector3(0, sizeY, 0);
			transform.Translate(newPos);
			open = true;
		}
	}

	public void Close()
	{
		if(open)
		{
			Vector3 newPos = new Vector3(0, -sizeY, 0);
			transform.Translate(newPos);
			open = false;
		}
	}
}
