using UnityEngine;
using System.Collections;

public class PlayerControllPhysics : RaycastController
{
	public CollisionInfo collisions;						// Holds the Collisions info
	public float skinWidth = .07f;							// Holds the SkinWidth of the collider, should be the same size as in Character Controller
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

		if(velocity.x != 0)
		{
			collisions.dirX = Mathf.Sign(velocity.x); // TODO Achar direçao que esta indo, talvez pelo collisionInfo
		}

		HorizontalCollisions(velocity);
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

	void VerticalCollisions(ref Vector3 velocity)
	{

	}

	public struct CollisionInfo
	{
		public bool left, right;							// Stores whether there is a collision at left or right

		public float dirX;									// Stores the direction the player is facing

		public void Reset()
		{
			left = false;
			right = false;
		}
	}
}
