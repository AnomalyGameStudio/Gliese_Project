using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformController : RaycastController 
{
	public LayerMask passengerMask;											// The LayerMask to find the passengers
	
	[Range (0,2)] public float easeAmount;									// The Amount used on the Ease Function [The optimal range is between 0 and 2]
	public float speed;														// The speed that the platform moves
	public float waitTime;													// The amount of time that the platform waits until next waypoint
	public bool cyclic;														// Stores if the waypoints should be cyclic

	public Vector3[] localWaypoints;										// Stores the waypoints the platform should move

	Vector3[] globalWaypoints;												// Stores the global position of the waypoints

	float nextMoveTime;														// Stores the time the platform should move next
	int fromWaypointIndex;
	float percentBetweenWaypoints;

	List<PassengerMovement> passengerMovement;								// Stores a List with all passengers
	Dictionary<Transform, PlayerControllPhysics> passengerDictionary;		// The Dictionary with the

	void Start()
	{
		// Check if there is any waypoint
		if(localWaypoints.Length < 1)
		{
			Debug.LogError("There is no waypoints");
		}
		else
		{
			globalWaypoints = new Vector3[localWaypoints.Length];

			// Stores the global position of the selected waypoints
			for(int i = 0; i < localWaypoints.Length; i++)
			{
				globalWaypoints[i] = localWaypoints[i] + transform.position;
			}
		}

		passengerDictionary = new Dictionary<Transform, PlayerControllPhysics> ();
	}
	// TODO Comment this method
	void Update()
	{
		UpdateRaycastOrigins();
		
		Vector3 velocity = CalculatePlatformMovement();

		//MovePassenger(true);
		transform.Translate(velocity);
		//MovePassenger(false);
	}

	// TODO Comment this method
	void MovePassenger(bool beforeMovePlatform)
	{
		foreach(PassengerMovement passenger in passengerMovement)
		{
			if(!passengerDictionary.ContainsKey(passenger.transform))
			{
				passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<PlayerControllPhysics> ());
			}
			
			if(passenger.moveBeforePlatform == beforeMovePlatform) 
			{
				Vector3 velocity = passenger.velocity;
				// TODO Add a move to the PlayerController class
				passengerDictionary[passenger.transform].Move(ref velocity);
				passenger.setVelocity(velocity);
			}
		}
	}

	//Function that makes a smooth movement for platforms. Better suited to run with a from 1 to 3
	float Ease(float x)
	{
		float a = easeAmount + 1;
		return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1-x, a));
	}

	Vector3 CalculatePlatformMovement()
	{
		if(Time.time < nextMoveTime)
		{
			// If it isn't time to move yet returns 0,0,0
			return Vector3.zero;
		}

		// TODO figure how this Works. It will find the correct waypointIndex somehow
		fromWaypointIndex %= globalWaypoints.Length;
		// Same as Above
		int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;

		// Calculates the distance between waypoints
		float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
		// Calculate the interpolant for the lerp and is also the percent of the path already traveled
		percentBetweenWaypoints += Time.deltaTime * speed/distanceBetweenWaypoints;
		// Makes sure the value is between 0 and 1
		percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
		// Apply the Ease Function
		float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

		// Calculate the new Position based on a Linear interpolation
		Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

		// Check if the platform already reached the Waypoint
		if(percentBetweenWaypoints >= 1)
		{
			// Reset the percent travelled
			percentBetweenWaypoints = 0;
			// Go to the next Waypoint
			fromWaypointIndex ++;

			// Check wheter the waypoints should be cyclic
			if(!cyclic)
			{
				// Check if the last waypoint was reached
				if(fromWaypointIndex >= globalWaypoints.Length-1)
				{
					// Reset the current waypoint index
					fromWaypointIndex = 0;

					// Reverse the order of the waypoints
					System.Array.Reverse(globalWaypoints);
				}
			}
			// Sets the time it should move next
			nextMoveTime = Time.time + waitTime;
		}

		// Returns the distance it should move
		return newPos - transform.position;
	}

	void CalculatePassengerMovement(Vector3 velocity)
	{
		// The ray being cast
		Ray ray;
		// The hit of a ray
		RaycastHit hit;

		// Stores the passengers on the platform that have already moved
		HashSet<Transform> movedPassengers = new HashSet<Transform>();

		// Initialize the passenger movement list
		passengerMovement = new List<PassengerMovement>();

		// Find the direction the platform is moving
		float directionX = Mathf.Sign(velocity.x);
		float directionY = Mathf.Sign(velocity.y);

		// Vertically moving platform
		if(velocity.y != 0)
		{
			// The size of the ray that will be cast
			float rayLenght = Mathf.Abs(velocity.y) * skinWidth;

			// Creates the RayCast
			for(int i = 0; i < verticalRayCount; i++)
			{
				// Select the origin of the rays based on the moving direction
				Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
				// Adds the spacing to the ray position
				rayOrigin += Vector2.right * (verticalRaySpacing * i);

				// Creates the ray
				ray = new Ray(rayOrigin, Vector2.up * directionY);

				// Casts the ray and check the hit
				if(Physics.Raycast(ray, out hit, rayLenght, passengerMask) && hit.distance != 0)
				{
					// Check if not already moved
					if(!movedPassengers.Contains(hit.transform))
					{
						// Adds the passenger to the moved list
						movedPassengers.Add(hit.transform);

						// Calculate the amount the passenger should move
						float pushX = (directionY == 1) ? velocity.x : 0;
						float pushY = velocity.y - (hit.distance - rayLenght) * directionY;

						// Adds the passenger to the to be moved List
						passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), (directionY == 1), true));
					}
				}
			}
		}

		// Horizontally moving Platform
		if(velocity.x != 0)
		{
			// The size of the ray that will be cast
			float rayLenght = Mathf.Abs(velocity.x) + skinWidth;

			// Creates the RayCast
			for(int i = 0; i < horizontalRayCount; i++)
			{
				// Select the origin of the rays based on the moving direction
				Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
				// Adds the spacing to the ray position
				rayOrigin += Vector2.up * (horizontalRaySpacing * i);

				// Creates the ray
				ray = new Ray(rayOrigin, Vector2.right * directionX);

				// Casts the ray and check the hit
				if(Physics.Raycast(ray, out hit, rayLenght, passengerMask) && hit.distance != 0)
				{
					// Check if not already moved
					if(!movedPassengers.Contains(hit.transform))
					{
						// Adds the passenger to the moved list
						movedPassengers.Add(hit.transform);

						// Calculate the amount the passenger should move
						float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
						float pushY = -skinWidth;

						// Adds the passenger to the to be moved List
						passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
					}
				}
			}
		}

		//Passenger on horizontally or downward moving platform
		if(directionY == -1 || velocity.y == 0 && velocity.x != 0)
		{
			// The size of the ray that will be cast
			float rayLength = skinWidth * 2;

			// Creates the RayCast
			for(int i = 0; i < verticalRayCount; i++)
			{
				// Select the origin of the rays based on the moving direction
				Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);

				// Creates the ray
				ray = new Ray(rayOrigin, Vector2.up);

				// Casts the ray and check the hit
				if(Physics.Raycast(ray, out hit, rayLength, passengerMask) && hit.distance != 0)
				{
					// Check if not already moved
					if(!movedPassengers.Contains(hit.transform))
					{
						// Adds the passenger to the moved list
						movedPassengers.Add(hit.transform);

						// Calculate the amount the passenger should move
						float pushX = velocity.x;
						float pushY = velocity.y;

						// Adds the passenger to the to be moved List
						passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
					}
				}
			}
		}
	}

	struct PassengerMovement
	{
		public Transform transform;
		public Vector3 velocity;
		public bool standingOnPlatform;
		public bool moveBeforePlatform;
		
		public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform)
		{
			transform = _transform;
			velocity = _velocity;
			standingOnPlatform = _standingOnPlatform;
			moveBeforePlatform = _moveBeforePlatform;
		}

		public void setVelocity(Vector3 _velocity)
		{
			velocity = _velocity;
		}
	}

	void OnDrawGizmos()
	{
		// If there is waypoints it creates Gizmos of the waypoint
		if(localWaypoints != null)
		{
			Gizmos.color = Color.green;
			float size = .3f;
			
			for (int i = 0; i < localWaypoints.Length; i++)
			{
				// Check if playing and use the waypoint accordingly
				Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;

				// Draws a cross on the waypoint position
				Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
				Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
			}
		}
	}
}