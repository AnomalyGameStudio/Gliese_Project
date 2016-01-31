using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonHandler : MonoBehaviour 
{
	public DoorController door;												// Holds the Door to be opend

	List<Collider> onTop;													// Holds all objects that are on top of the button

	void Start()
	{
		// Initialize the onTop list
		onTop = new List<Collider>();
	}

	void OnTriggerEnter(Collider collider)
	{
		// Check if the collider is a player or a draggable
		if(collider.tag == "Player" || collider.tag == "Draggable")
		{
			// In case there isn't already another object on top
			if(onTop.Count < 1)
			{
				// Signals to open
				door.ToggleDoor(true);
			}

			// If not already ont the onTop list
			if(!onTop.Contains(collider))
			{
				// Add to the list
				onTop.Add(collider);
			}

		}
	}
	
	void OnTriggerExit(Collider collider)
	{
		// When exiting the collider, remove from the list
		onTop.Remove(collider);

		// If there is no object on top of the button
		if(onTop.Count < 1)
		{
			// Closes the door
			door.ToggleDoor(false);
		}
	}
}
