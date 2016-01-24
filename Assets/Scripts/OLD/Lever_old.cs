using UnityEngine;
using System.Collections;

public class Lever_old : MonoBehaviour
{
	public Door door;
	public bool activated;

	void Start()
	{

	}

	void OnTriggerStay(Collider collider)
	{
		bool actionButton = Input.GetButtonDown("Action");

		if(collider.tag == "Player" && actionButton)
		{
			if(!activated)
			{
				door.Open();
				activated = true;
			}
			else
			{
				door.Close();
				activated = false;
			}
		}
	}
}
