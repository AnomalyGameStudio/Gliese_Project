using UnityEngine;
using System.Collections;

public class PlayerControllPhysics : RaycastController
{
	public CollisionInfo collisions;						// Holds the Collisions info
	public float skinWidth = .07f;							// Holds the SkinWidth of the collider, should be the same size as in Character Controller

	float maxDescendAngle = 75;

	void Start ()
	{
		base.Start ();										// Runs the Start function of the parent class
		collisions = new CollisionInfo();					// Instantiate the CollisionInfo struct
	}

	/*
	void Update()
	{
		//Vector3 velocity = new Vector3 (1f,1f,0f);
		//CheckCollider(ref velocity);
	}
	*/

	public void CheckCollider(ref Vector3 velocity)
	{
		UpdateRaycastOrigins();								// Updates the position of the Raycast
		collisions.Reset();

		//Debug.Log("bottomleft: " + raycastOrigins.bottomLeft + " bottomright: " + raycastOrigins.bottomRight);
		//Debug.Log("topleft: " + raycastOrigins.topLeft + " topright: " + raycastOrigins.topRight);

		if(velocity.x != 0)
		{
			collisions.dirX = Mathf.Sign(velocity.x); // TODO Achar direçao que esta indo, talvez pelo collisionInfo
		}

		HorizontalCollisions(velocity);

		if(velocity.y < 0)
		{
			DescendSlope(ref velocity);
		}
	}

	void HorizontalCollisions(Vector3 velocity)
	{
		float rayLength = Mathf.Abs(velocity.x) + skinWidth;
		
		if(Mathf.Abs(velocity.x) < skinWidth)
		{
			rayLength = 2 * skinWidth;
		}
		
		for(int i = 0; i < horizontalRayCount; i++)
		{
			Vector2 rayOrigin = (collisions.dirX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			
			RaycastHit hit;
			Ray ray = new Ray(rayOrigin, Vector2.right * collisions.dirX);
			
			Debug.DrawRay(ray.origin, ray.direction, Color.green);
			
			if(Physics.Raycast(ray, out hit, rayLength, collisionMask))
			{
				if(hit.distance < (2 * skinWidth))
				{
					collisions.left = collisions.dirX == -1;
					collisions.right = collisions.dirX == 1;
					continue;
				}
			}
		}
	}

	// TODO Verificar se ha necessidade dessa rotina
	void VerticalCollisions(ref Vector3 velocity)
	{
		float directionY = Mathf.Sign(velocity.y);
		float rayLength = Mathf.Abs(velocity.y) + skinWidth;

		for(int i = 0; i < verticalRayCount; i++)
		{
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i);

			RaycastHit hit;
			Ray ray = new Ray(rayOrigin, Vector2.up * directionY);

			Debug.DrawRay(ray.origin, ray.direction, Color.green);
			
			if(Physics.Raycast(ray, out hit, rayLength, collisionMask))
			{
				/*
				if(hit.collider.tag == "Through")
				{
					if(directionY == 1 || hit.distance == 0)
					{
						continue;
					}
					
					if(collisions.fallingThroughPlatform)
					{
						continue;
					}
					
					if(playerInput.y == -1)
					{
						collisions.fallingThroughPlatform = true;
						Invoke("resetFallingThroughPlatform", .5f);
						continue;
					}
				}
				*/
				if(controller.isGrounded)
				{
					velocity.y = (hit.distance - skinWidth) * directionY;
					rayLength = hit.distance;
				}

				/*
				if(collisions.climbingSlope)
				{
					velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
				}
				*/
				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}
	}

	// TODO Not falling fast enought on slopes
	void DescendSlope(ref Vector3 velocity)
	{
		float directionX = Mathf.Sign(velocity.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight: raycastOrigins.bottomLeft;
		//RaycastHit hit = Physics.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);
		
		RaycastHit hit;
		Ray ray = new Ray(rayOrigin, -Vector2.right * directionX);
		Physics.Raycast(ray, out hit, Mathf.Infinity, collisionMask);
		
		Debug.DrawRay(ray.origin, ray.direction, Color.green);
		
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, collisionMask))
		{
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			//Debug.Log(slopeAngle);
			if(slopeAngle != 0 && slopeAngle <= maxDescendAngle)
			{
				if(Mathf.Sign(hit.normal.x) == directionX)
				{
					if(hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
					{

						//velocity.y += -111;
						//Debug.Log(velocity.y);
						/*
						float moveDistance = Mathf.Abs(velocity.x);
						float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
						velocity.y -= descendVelocityY;


						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
						*/
					}
				}
			}
		}
	}

	public struct CollisionInfo
	{
		public bool left, right;							// Stores whether there is a collision at left or right
		public bool below, above;							// Stores whether there is a collision on top or bottom

		public float dirX;									// Stores the direction the player is facing

		public void Reset()
		{
			left = false;
			right = false;
			below = false;
			above = false;
		}
	}
}
