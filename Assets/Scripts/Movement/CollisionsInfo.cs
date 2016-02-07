using UnityEngine;
using System.Collections;

public class CollisionsInfo : ICollisionInfo
{
	// Colision Info
	public bool above, below;											// Holds if it is colliding with anything above or Bellow
	public bool left, right;											// Holds if it is colliding with anything on the sides
	
	// Slopes Info
	public bool climbingSlope, descendingSlope;							// Holds if the player is climbing or descending a slope
	public float slopeAngle, slopeAngleOld;								// Holds the angle of the current Slope and the Last Angle stored
	
	// Player Info
	public bool fallingThroughPlatform;									// Holds if the player is falling through a platform
	public bool jump;													// Holds if player is Jumping
	public bool doubleJump;												// Tells if the player is able to do a double jump
	public bool draging;												// Tells if the player is dragging any draggable
	public int faceDir; 												// Direction the character is facing
	public Vector3 velocityOld;											// The velocity of the player at the start of the Move Method
	
	// Resets the collisions
	public void Reset()
	{
		above = below = false;
		left = right = false;
		climbingSlope = descendingSlope = false;
		
		slopeAngleOld = slopeAngle;
		slopeAngle = 0;
	}
	
	// TODO implement Crouch
	// Used to the Crouch and Slide

	public bool crouch;
	public Vector3 originalSize;
	public Vector3 originalCenter;
	public float colliderScale;
	public BoxCollider collider;
	private Vector3 s;
	private Vector3 c;

	public void SetCollider(Vector3 size, Vector3 center)
	{
		collider.size = size;
		collider.center = center;
			
		s = size * colliderScale;
		c = center * colliderScale;
	}
		
	public void ResetCollider()
	{
		SetCollider(originalSize, originalCenter);
	}
}
