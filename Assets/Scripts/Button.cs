using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour 
{
	public Door door;
	public bool keepOpen;

	void OnTriggerEnter(Collider collider)
	{
		if(collider.tag == "Player" || collider.tag == "Draggable")
		{
			door.Open();
		}
	}

	void OnTriggerExit()
	{
		if(!keepOpen)
		{
			door.Close();
		}
	}
}
