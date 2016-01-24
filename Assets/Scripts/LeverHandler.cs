using UnityEngine;
using System.Collections;

public class LeverHandler : MonoBehaviour 
{
	public PlatformController platform;

	void OnTriggerStay(Collider collider)
	{
		// Gets the Action Button
		bool actionButton = Input.GetButtonDown("Action");

		// If the player is in the collider and the action button is pressed
		if(collider.tag == "Player" && actionButton)
		{
			// Signals the platform to move to the next platform
			platform.ManualMove();
		}
	}
}
