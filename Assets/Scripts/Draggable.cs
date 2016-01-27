using UnityEngine;
using System.Collections;

public class Draggable : PlayerPhysicsImproved
{
	// TODO Still not working
	void Start () 
	{
		base.Start ();
	}

	void Update()
	{
		Vector3 velocity = Vector3.zero;
		velocity.y += GameController.instance.gravity * Time.deltaTime;
		Move(velocity);
	}
}
