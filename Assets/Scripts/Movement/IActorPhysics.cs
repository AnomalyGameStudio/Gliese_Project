using UnityEngine;
using System.Collections;

public interface IActorPhysics 
{
	void Move(Vector3 velocity, Vector2 input, bool standingOnPlatform);

	void HorizontalCollisions(Vector3 velocity);

	void VerticalCollisions(Vector3 velocity);

	void ClimbSlope(Vector3 velocity, float slopeAngle);

	void DescendSlope(Vector3 velocity);

	void ResetFallingThroughPlatform();
}
