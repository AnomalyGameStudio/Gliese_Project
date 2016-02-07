using UnityEngine;
using System.Collections;

public class Draggable : PlayerPhysicsImproved
{
	Vector3 velocity;

	void Start () 
	{
		base.Start ();
	}

	void Update()
	{
		if(!collisions.below)
		{
			velocity.y += GameController.instance.gravity * Time.deltaTime;
			Move(velocity * Time.deltaTime);
		}

		if(collisions.below || collisions.above)
		{
			velocity.y = 0;
		}
	}
}
