using UnityEngine;
using System.Collections;

public interface ICollisionInfo 
{
	// This method should reset the basic collisions
	void Reset();

	// This method will store a collider
	void SetCollider(Vector3 size, Vector3 center);

	// This method will reset a collider to a previous state
	void ResetCollider();
}
