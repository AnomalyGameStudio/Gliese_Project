using UnityEngine;
using System.Collections;

public class Draggable : RaycastController 
{
	GameController gameController;
	float maxClimbAngle = 80;
	float maxDescendAngle = 75;
	
	public CollisionInfo collisions;
	Vector3 velocity;

	void Start () 
	{
		base.Start ();
		gameController = GameController.instance;
	}

	void Update()
	{
		velocity.y += gameController.gravity * Time.deltaTime;
		Move(velocity * Time.deltaTime);
	}

	public void Move(Vector3 velocity)
	{
		UpdateRaycastOrigins();

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
		
		public void Reset()
		{
			above = below = false;
			left = right = false;
			climbingSlope = descendingSlope = false;
			
			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}
}
