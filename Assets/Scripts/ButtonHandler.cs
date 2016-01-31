using UnityEngine;
using System.Collections;

public class ButtonHandler : MonoBehaviour 
{
	public DoorController door;

	/*
	void OnTriggerStay(Collider collider)
	{
		// Gets the Action Button
		bool actionButton = Input.GetButtonDown("Action");
		
		// If the player is in the collider and the action button is pressed
		if(collider.tag == "Player" && actionButton)
		{
			// Signals the platform to move to the next platform
			door.ManualMove();
		}
	}
*/

	void OnTriggerEnter(Collider collider)
	{
		if(collider.tag == "Player" || collider.tag == "Draggable")
		{
			door.ManualMove(false);
		}
	}
	
	void OnTriggerExit()
	{
			door.ManualMove(true);
	}

}
