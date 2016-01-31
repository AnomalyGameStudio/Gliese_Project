using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DoorController : RaycastController 
{
	[Range (0,2)] public float easeAmount;									// The Amount used on the Ease Function [The optimal range is between 0 and 2]
	public float speed;														// The speed that the platform moves
	public float waitTime;													// The amount of time that the platform waits until next waypoint
	public bool cyclic;														// Stores if the waypoints should be cyclic
	public bool automaticMovement = false;									// Controlls if the waypoints should be executed automatic or Manually
	public Vector3[] localWaypoints;										// Stores the waypoints the platform should move
	
	Vector3[] globalWaypoints;												// Stores the global position of the waypoints
	
	float nextMoveTime;														// Stores the time the platform should move next
	int fromWaypointIndex;
	float percentBetweenWaypoints;

	// Teste
	bool moving;
	bool shouldMove;
	bool manualMove;

	// Teste 2
	Vector3 positionFrom;

	void Start()
	{
		base.Start();
		
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
	}

	void Update()
	{
		// Update the position of the Raycast Origins
		UpdateRaycastOrigins();
		// Calculate the velocity the platform should move
		Vector3 velocity = CalculatePlatformMovement();


		// Move the platform
		transform.Translate(velocity);
	}

	// Flags a manual move on the platform
	public void ManualMove(bool next)
	{
		if(next)
		{
			fromWaypointIndex++;
		}
		positionFrom = transform.position;
		manualMove = true;
	}

	// Determine if the platform should move
	bool ShouldMove()
	{
		return (automaticMovement || moving || manualMove);
	}

	float Ease(float x)
	{
		float a = easeAmount + 1;
		return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1-x, a));
	}

	Vector3 CalculatePlatformMovement()
	{
		if(Time.time < nextMoveTime || !ShouldMove())
		{
			// If it isn't time to move yet returns 0,0,0
			return Vector3.zero;
		}
		
		// This will make sure that the list will restart
		fromWaypointIndex %= globalWaypoints.Length;
		
		// this will set the waypoint to go
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
		
		// Flags that the platform is moving
		moving = true;
		
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
			
			// Ends the manual movement
			moving = false;
			manualMove = false;
			
			// Sets the time it should move next
			nextMoveTime = Time.time + waitTime;
		}
		
		// Returns the distance it should move
		return newPos - transform.position;
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
