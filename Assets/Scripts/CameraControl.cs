using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour 
{
	public PlayerController target;					// The target the camera will be pointing

	public Vector2 focusAreaSize;					// The size of the focus Area.

	public float verticalOffset;					// The vertical offset of the camera
	public float lookAheadDistanceX;				// The distance the camera should look ahead
	public float lookSmoothTimeX;					// The acceleration of the camera on the look ahead
	public float verticalSmoothTime;				// The vertical acceleration of the focus area

	FocusArea focusArea;							// The area the camera should focus

	float lookAheadDirX;							// Direction to look ahead
	float targetLookAheadX;							// The target look ahead

	float currentLookAheadX; 	// TODO Find out WTF is this?
	float smoothLookVelocityX;	// TODO Find out WTF is this?
	float smoothVelocityY;		// TODO Find out WTF is this?

	bool lookAheadStopped; 		// TODO Find out WTF is this?

	void Start()
	{
		focusArea = new FocusArea(target.controller.bounds, focusAreaSize);
	}

	void LateUpdate()
	{
		if(!target) return;

		focusArea.Update(target.controller.bounds);

		Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

		if(focusArea.velocity.x != 0)
		{
			/*
			lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
			if(Mathf.Sign(target.playerInput.x) == lookAheadDirX && target.playerInput.x != 0)
			{
				lookAheadStopped = false;
				targetLookAheadX = lookAheadDirX * lookAheadDistanceX;
			}
			else if( !lookAheadStopped)
			{
				lookAheadStopped = true;
				targetLookAheadX = currentLookAheadX * (lookAheadDirX * lookAheadDistanceX - currentLookAheadX) / 4f;
			}
			*/
		}

		currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

		focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
		focusPosition += Vector2.right * currentLookAheadX;
		
		transform.position = (Vector3) focusPosition + Vector3.forward * -30;

	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawCube(focusArea.center, focusAreaSize);		// Draw the focus Area
	}

	struct FocusArea
	{
		public Vector2 center;
		public Vector2 velocity;
		
		float left, right;
		float top, bottom;
		
		public FocusArea(Bounds targetBounds, Vector2 size)
		{
			left = targetBounds.center.x - size.x/2;
			right = targetBounds.center.x + size.x/2;
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + size.y;
			
			velocity = Vector2.zero;
			center = new Vector2((left+right)/2 , (top+bottom)/2);
		}

		public void Update(Bounds bounds)
		{
			float shiftX = 0;				// The amount the focus area should move in the X axys
			float shiftY = 0;				// The amount the focus area should move in the Y axys

			// Check the sides to see if the player reached it
			if (bounds.min.x < left)
			{
				shiftX = bounds.min.x - left;
			}
			else if (bounds.max.x > right)
			{
				shiftX = bounds.max.x - right;
			}

			// Move the focus area sideways
			right += shiftX;
			left += shiftX;

			// Check top and bottom to see if the player reached it
			if(bounds.min.y < bottom)
			{
				shiftY = bounds.min.y - bottom;
			}
			else if ( bounds.max.y > top)
			{
				shiftY = bounds.max.y - top;
			}

			// Move the focus area up or down
			bottom += shiftY;
			top += shiftY;

			// Setting the new Center of the focus area
			center = new Vector2((left+right)/2 , (top+bottom)/2);

			// Setting the velocity to move the focus area to the new center
			velocity = new Vector2(shiftX, shiftY);

			Debug.Log("Center: " + center + " velocity: " + velocity + " left: " + left + " right: " + right + " top: " + top + " bottom: " + bottom);
		}
	}
}
