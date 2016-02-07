using UnityEngine;
using System.Collections;

// Extends the Class RaycastController and implements the Interface IActorPhysics
public class PlayerPhysicsImproved : RaycastController, IActorPhysics
{
	public LayerMask dragMask;									// The layer that will be dragged by the player
	
	private CollisionsInfo playerCollisions;					// The information about the collision of the player
	private float maxClimbAngle = 80;							// The maximum Slope the player can climb
	private float maxDescendAngle = 75;							// The maximum Slope the player can descend before falling
	private RaycastHit hit;										// Stores the place the raycast hit
	private Ray ray;											// Stores the ray
	
	[HideInInspector] public Vector2 playerInput;				// The input from the player. TODO Find out if still needed
	
	#region Getters and Setters
	public CollisionsInfo collisions							
	{
		get
		{
			if(playerCollisions == null)
			{
				playerCollisions = new CollisionsInfo();
			}
			
			return playerCollisions;
		}
	}
	#endregion
	
	void Start ()
	{
		// Call the Start from the parent class
		base.Start ();
		// Set the direction the player is looking
		collisions.faceDir = 1;
		
		// Get the BoxCollider component from the Player
		//collisions.collider = GetComponent<BoxCollider> ();
		// Save the scale of the player
		//collisions.colliderScale = transform.localScale.x;
	}
	
	// Function Used to move using only the velocity
	public void Move(Vector3 velocity)
	{
		Move(velocity, false);
	}
	
	// The function used to move the player without the the player input
	public void Move(Vector3 velocity, bool standingOnPlatform)
	{
		Debug.Log("Called it: " + velocity);
		Move(velocity, Vector2.zero, standingOnPlatform);
	}
	
	public void Move(Vector3 velocity, Vector2 input, bool standingOnPlatform = false)
	{
		// Updates the origin position of the raycasts
		UpdateRaycastOrigins();
		// Reset the collisions
		collisions.Reset();
		// Stores the velocity
		collisions.velocityOld = velocity;
		// Stores the player input
		playerInput = input;
		
		// Check if moving to any side
		if(velocity.x != 0 && !collisions.draging)
		{
			// Updates the direction the player is looking
			collisions.faceDir = (int) Mathf.Sign(velocity.x);
		}
		
		// Check If the player is moving falling down
		if(velocity.y < 0)
		{
			// Calculate the velocity of descending a slope
			DescendSlope(ref velocity);
		}
		
		// Check the horizontal collisions
		HorizontalCollisions(ref velocity);
		
		// Check If the player is either jumping or falling down
		if(velocity.y != 0)
		{
			// Check the vertical collisions
			VerticalCollisions(ref velocity);
		}
		
		// Move the player. Second Argument is used so the player move relative to the world
		transform.Translate(velocity, Space.World);
		
		// If player is on a platform, updates the collisions
		if(standingOnPlatform)
		{
			collisions.below = true;
		}
	}
	
	#region Horizontal Collisions
	// Checks the horizontal collisions
	void HorizontalCollisions(ref Vector3 velocity)
	{
		// Stores the direction the player is facing
		float directionX = collisions.faceDir;
		// Calculate the Size of the Ray that will be cast
		float rayLength = Mathf.Abs(velocity.x) + skinWidth;
		
		// Sets the lenght of the ray to 2 times the skin in case the velocity is lesser than the Skin. (Used to detect the walls if jumping next to them)
		if(Mathf.Abs(velocity.x) < skinWidth)
		{
			rayLength = 2 * skinWidth;
		}
		
		// Will cast every one of the horizontal rays
		for(int i = 0; i < horizontalRayCount; i++)
		{
			// Gets the origin according to the direction
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			// Will move the position according to the number of the ray
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			
			// Create the ray
			ray = new Ray(rayOrigin, Vector2.right * directionX);
			
			// Shows the Ray gizmo
			Debug.DrawRay(ray.origin, ray.direction, Color.green);
			
			if(tag != "Draggable" && Input.GetButton("Action"))
			{
				// TODO: Draggable Collisions. Maybe have to do here because of the bug with the X axis of the velocity
				if(Physics.Raycast(ray, out hit, rayLength, dragMask))
				{
					//if(hit.distance == 0)
					{
						//continue;
					}
					//velocity.x = (hit.distance - skinWidth) * directionX;
					
					Debug.Log("Dragging");
					Draggable draggable = hit.transform.GetComponent<Draggable> ();
					Vector3 dragVelocity = Vector3.zero;
					//dragVelocity.y += GameController.instance.gravity * Time.deltaTime;
					dragVelocity.x = velocity.x;
					draggable.Move(dragVelocity);
					collisions.draging = true;
					//continue;
				}
			}
			
			// Cast the Ray and see if it has hit
			if(Physics.Raycast(ray, out hit, rayLength, collisionMask))
			{
				// If the distance of the hit is 0 go to the next ray
				if(hit.distance == 0)
				{
					continue;
				}
				
				if(hit.transform.tag == "Draggable" && Input.GetButton("Action"))
				{
					/*
					Debug.Log("Dragging");
					Draggable draggable = hit.transform.GetComponent<Draggable> ();
					Vector3 dragVelocity = Vector3.zero;
					//dragVelocity.y += GameController.instance.gravity * Time.deltaTime;
					dragVelocity.x = velocity.x;
					draggable.Move(dragVelocity);
					collisions.draging = true;
					*/
				}
				else
				{
					collisions.draging = false;
				}
				
				// determine the Angle of the object relative to the player
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				
				// If it is the first ray and the angle of the slope is lesser than the threshold
				if (i == 0 && slopeAngle <= maxClimbAngle)
				{
					// If it is descending slope
					if(collisions.descendingSlope)
					{
						// Reset the descending slope flag
						collisions.descendingSlope = false;
						// Restore the Old velocity
						velocity = collisions.velocityOld;
					}
					
					// Holds the distance to the slope
					float distanceToSlopeStart = 0;
					
					// See if the angle of the slope is different than the previous angle
					if(slopeAngle != collisions.slopeAngleOld)
					{
						// Updates the distance until the slope
						distanceToSlopeStart = hit.distance - skinWidth;
						// Reduces the velocity on the X axis
						velocity.x -= distanceToSlopeStart * directionX;
					}
					
					// Call the Climb Slope routine
					ClimbSlope(ref velocity, slopeAngle);
					// Then Add the distance to the slope to the X axis of the velocity
					velocity.x += distanceToSlopeStart * directionX;
				}
				
				// Not climbing a slope or the slope is bigger than the maximum threshold
				if(!collisions.climbingSlope || slopeAngle > maxClimbAngle)
				{
					// Recalculate the velocity on the X axis
					velocity.x = (hit.distance - skinWidth) * directionX; //TODO Bug: Ao chegar num slope que nao pode subir, ele fica pulando feito um maluco
					// Change the raylenght to the distance
					rayLength = hit.distance;
					
					// Check if it is climbing a slope
					if(collisions.climbingSlope)
					{
						// Change the velocity on the Y axis to the Tangent of the Angle times the velocity on the X axis
						velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
					}
					
					// Determine the collisions
					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				}
			}
		}
	}
	#endregion
	
	#region Vertical Collision
	// Checks the vertical collisions
	void VerticalCollisions(ref Vector3 velocity)
	{
		// Stores the direction in the Y axis
		float directionY = Mathf.Sign(velocity.y);
		// Claculate the Length of the ray to be cast
		float rayLength = Mathf.Abs(velocity.y) + skinWidth;
		
		// Cast the Ray and see if it has hit
		for(int i = 0; i < verticalRayCount; i++)
		{
			// Gets the origin according to the direction
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			// Will move the position according to the number of the ray
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			
			// Create the ray
			ray = new Ray(rayOrigin, Vector2.up * directionY);
			
			// Draw a ray Gizmo
			Debug.DrawRay(ray.origin, ray.direction, Color.green);
			
			// Cast the ray and see if it has hit
			if(Physics.Raycast(ray, out hit, rayLength, collisionMask))
			{
				// Check if the Tag of the Collider is Throught
				if(hit.collider.tag == "Through")
				{
					// If the direction is 1 (Up) or the distance is 0 goes to next ray
					if(directionY == 1 || hit.distance == 0)
					{
						continue;
					}
					
					// If it is falling through the platform goes to the next ray
					if(collisions.fallingThroughPlatform)
					{
						continue;
					}
					
					// If player inputx -1 (down) TODO Find out how to work with a controller
					if(playerInput.y == -1)
					{
						// Set falling through platform flag
						collisions.fallingThroughPlatform = false;
						
						// Invoke the co routine ResetFallingThroughPlatform .5 seconds later and goes to the next ray
						Invoke("resetFallingThroughPlatform", .5f);
						continue;
					}
				}
				
				// Calculates the new velocity on the Y axis
				velocity.y = (hit.distance - skinWidth) * directionY;
				// Calculates the length of the ray
				rayLength = hit.distance;
				
				// Check if climbing a slope and calculate the X velocity
				if(collisions.climbingSlope)
				{
					velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
				}
				
				// Set collisions flags
				collisions.jump = false;
				collisions.doubleJump = true;
				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}
		
		// Check if it is climbing a slope
		if(collisions.climbingSlope)
		{
			// Check the direction of the player. TODO Update this to the collisions info
			float directionX = Mathf.Sign(velocity.x);
			// Calculates the length of the ray
			rayLength = Mathf.Abs(velocity.x) + skinWidth;
			// Gets the origin according to the direction TODO WTF?
			Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
			
			// Create the ray
			ray = new Ray(rayOrigin, Vector2.right * directionX);
			
			// Shows the ray gizmo
			Debug.DrawRay(ray.origin, ray.direction, Color.green);
			
			// Casts the ray and see if it has a hit
			if(Physics.Raycast(ray, out hit, rayLength, collisionMask))
			{
				// Determine the Angle of the slope
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				// Check if the Angle is different of the one stored on the Collisions info
				if(slopeAngle != collisions.slopeAngle)
				{
					// Updates the Velocity in the X axis and the Slope Angle
					velocity.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}	
		}
	}
	#endregion
	
	#region ClimbSlope
	// Calculates the velocity if the player is climbing a slope
	void ClimbSlope(ref Vector3 velocity, float slopeAngle)
	{
		// Determine the distance the player will move
		float moveDistance = Mathf.Abs(velocity.x);
		// Calculate the velocity on the Y axis the player will climb
		float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
		
		// If the current Velocity in the Y is lesser the climbVelocity
		if(velocity.y <= climbVelocityY)
		{
			// Updates the velocity on both Axis
			velocity.y = climbVelocityY;
			velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
			
			// Update the collisions info
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}
	}
	#endregion
	
	#region DescendSlope
	void DescendSlope(ref Vector3 velocity)
	{
		// Stores the direction in the Y axis
		float directionX = Mathf.Sign(velocity.x);
		// Gets the origin contrary to the direction
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight: raycastOrigins.bottomLeft;
		
		// Create the ray
		ray = new Ray(rayOrigin, -Vector2.right * directionX);
		
		// Shows the ray gizmo
		Debug.DrawRay(ray.origin, ray.direction, Color.green);
		
		// Casts the ray and see if it has a hit
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, collisionMask))
		{
			// Stores the angle of the slope
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			// Check if the slope is not 0 and the Max Descend angle
			if(slopeAngle != 0 && slopeAngle <= maxDescendAngle)
			{
				// Check if the hit is in the direction the player is going
				if(Mathf.Sign(hit.normal.x) == directionX)
				{
					if(hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
					{
						// Sets the moveDistance
						float moveDistance = Mathf.Abs(velocity.x);
						// Takes the curveModifier and calculate the descend velocity
						float descendVelocityY = SlopeCurveModifier.Evaluate(slopeAngle);
						// The velocity the player should move
						velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
						velocity.y -= descendVelocityY;
						
						// Update the collisions info
						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
			}
		}
	}
	#endregion
	
	// Method used to reset the Faling Through Platform Flag by an invoke with a delay
	void ResetFallingThroughPlatform()
	{
		collisions.fallingThroughPlatform = false;
	}
}
