using UnityEngine;
using System.Collections;

public class PlayerPhysicsImproved : RaycastController 
{
	float maxClimbAngle = 80;
	float maxDescendAngle = 75;

	public CollisionInfo collisions;

	[HideInInspector]
	public Vector2 playerInput;

	void Start () 
	{
		base.Start ();
		collisions.faceDir = 1;
		collisions.collider = GetComponent<BoxCollider> ();

		collisions.originalSize = collider.size;
		collisions.originalCenter = collider.center;
		collisions.colliderScale = transform.localScale.x;
	}

	public void Move(Vector3 velocity, bool standingOnPlatform)
	{
		Move(velocity, Vector2.zero, standingOnPlatform);
	}

	public void Move(Vector3 velocity, Vector2 input, bool standingOnPlatform = false)
	{
		UpdateRaycastOrigins();
		collisions.Reset();
		collisions.velocityOld = velocity;

		playerInput = input;
		
		if(velocity.x != 0)
		{
			collisions.faceDir = (int) Mathf.Sign(velocity.x);

			//Face direction
			if (collisions.faceDir != 0)// && !wallHolding)
			{
				transform.eulerAngles = (collisions.faceDir > 0) ? Vector3.zero : Vector3.up * 180;
			}
		}
		else
		{
			if(input.y < 0)
			{
				//crouch();
			}
			else
			{
				if(collisions.crouch)
				{
					velocity.y = -1;
					//collisions.ResetCollider();
					//collisions.crouch = false;
				}
			}
		}
		
		if(velocity.y < 0)
		{
			DescendSlope(ref velocity);
		}
		
		HorizontalCollisions(ref velocity);
		
		if(velocity.y != 0)
		{
			VerticalCollisions(ref velocity);
		}
		
		transform.Translate(velocity, Space.World);
		
		if(standingOnPlatform)
		{
			collisions.below = true;
		}

	}

	//TODO Add animation
	//TODO BUG: Player falling through the platform
	void crouch()
	{
		collisions.SetCollider(new Vector3(.74f, 1.25f, 1), new Vector3(.16f, -0.27f, 0));
		collisions.crouch = true;
		//transform.localScale = new Vector3(1, .5f, 1);
	}

	void HorizontalCollisions(ref Vector3 velocity)
	{
		float directionX = collisions.faceDir;
		float rayLength = Mathf.Abs(velocity.x) + skinWidth;

		if(Mathf.Abs(velocity.x) < skinWidth)
		{
			rayLength = 2 * skinWidth;
		}

		for(int i = 0; i < horizontalRayCount; i++)
		{
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);

			//RaycastHit hit = Physics.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			RaycastHit hit;
			Ray ray = new Ray(rayOrigin, Vector2.right * directionX);

			
			Debug.DrawRay(ray.origin, ray.direction, Color.green);

			if(Physics.Raycast(ray, out hit, rayLength, collisionMask))
			{
				if(hit.distance == 0)
				{
					continue;
				}

				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				
				if (i == 0 && slopeAngle <= maxClimbAngle)
				{
					if(collisions.descendingSlope)
					{
						collisions.descendingSlope = false;
						velocity = collisions.velocityOld;
					}
					
					float distanceToSlopeStart = 0;
					if(slopeAngle != collisions.slopeAngleOld)
					{
						distanceToSlopeStart = hit.distance - skinWidth;
						velocity.x -= distanceToSlopeStart * directionX;
					}
					
					ClimbSlope(ref velocity, slopeAngle);
					velocity.x += distanceToSlopeStart * directionX;
				}
				
				if(!collisions.climbingSlope || slopeAngle > maxClimbAngle)
				{
					velocity.x = (hit.distance - skinWidth) * directionX; //TODO Bug: Ao chegar num slope que nao pode subir, ele fica pulando feito um maluco
					rayLength = hit.distance;
					
					if(collisions.climbingSlope)
					{
						
						velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
					}
					
					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				}
			}
		}
	}

	void VerticalCollisions(ref Vector3 velocity)
	{
		float directionY = Mathf.Sign(velocity.y);
		float rayLength = Mathf.Abs(velocity.y) + skinWidth;
		
		for(int i = 0; i < verticalRayCount; i++)
		{
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);

			//RaycastHit hit = Physics.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			RaycastHit hit;
			Ray ray = new Ray(rayOrigin, Vector2.up * directionY);

			
			Debug.DrawRay(ray.origin, ray.direction, Color.green);

			if(Physics.Raycast(ray, out hit, rayLength, collisionMask))
			{
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
				
				velocity.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;
				
				if(collisions.climbingSlope)
				{
					velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
				}
				
				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}

		if(collisions.climbingSlope)
		{


			float directionX = Mathf.Sign(velocity.x);
			rayLength = Mathf.Abs(velocity.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;

			RaycastHit hit;
			Ray ray = new Ray(rayOrigin, Vector2.right * directionX);


			Debug.DrawRay(ray.origin, ray.direction, Color.green);

			if(Physics.Raycast(ray, out hit, rayLength, collisionMask))
			{
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				if(slopeAngle != collisions.slopeAngle)
				{
					velocity.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}	
		}
	}

	void ClimbSlope(ref Vector3 velocity, float slopeAngle)
	{
		float moveDistance = Mathf.Abs(velocity.x);
		float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
		
		if(velocity.y <= climbVelocityY)
		{
			velocity.y = climbVelocityY;
			velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
			
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}
	}

	void DescendSlope(ref Vector3 velocity)
	{
		float directionX = Mathf.Sign(velocity.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight: raycastOrigins.bottomLeft;
		//RaycastHit hit = Physics.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		RaycastHit hit;
		Ray ray = new Ray(rayOrigin, -Vector2.up * directionX);
		Physics.Raycast(ray, out hit, Mathf.Infinity, collisionMask);
		
		Debug.DrawRay(ray.origin, ray.direction, Color.green);

		if(Physics.Raycast(ray, out hit, Mathf.Infinity, collisionMask))
		{
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if(slopeAngle != 0 && slopeAngle <= maxDescendAngle)
			{
				if(Mathf.Sign(hit.normal.x) == directionX)
				{
					if(hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
					{
						float moveDistance = Mathf.Abs(velocity.x);
						float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
						velocity.y -= descendVelocityY;
						
						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
			}
		}
	}

	void resetFallingThroughPlatform()
	{
		collisions.fallingThroughPlatform = false;
	}
	
	public struct CollisionInfo
	{
		public bool above, below;
		public bool left, right;
		public bool climbingSlope, descendingSlope;
		public bool fallingThroughPlatform;
		public bool crouch;
		public float slopeAngle, slopeAngleOld;
		
		public int faceDir; //Direction the character is facing
		
		public Vector3 velocityOld;

		public Vector3 originalSize;
		public Vector3 originalCenter;
		public float colliderScale;

		public BoxCollider collider;
		//private Vector3 s;
		//private Vector3 c;

		public void Reset()
		{
			above = below = false;
			left = right = false;
			climbingSlope = descendingSlope = false;
			
			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}

		public void SetCollider(Vector3 size, Vector3 center)
		{
			collider.size = size;
			collider.center = center;
			
			//s = size * colliderScale;
			//c = center * colliderScale;
		}
		
		public void ResetCollider()
		{
			SetCollider(originalSize, originalCenter);
		}
	}
}
