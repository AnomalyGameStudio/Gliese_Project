using UnityEngine;
using System.Collections;

public interface IActorPhysics 
{
	CollisionsInfo collisions							
	{
		get;
	}

	void Move(Vector3 velocity);

	void Move(Vector3 velocity, bool standingOnPlatform);

	void Move(Vector3 velocity, Vector2 input, bool standingOnPlatform = false);
}
