﻿using UnityEngine;
using System.Collections;

public class CameraControl_OLD : MonoBehaviour 
{
	public PlayerController target;
	public Vector2 focusAreaSize;

	public float verticalOffset;
	public float lookAheadDstX;
	public float lookSmoothTimeX;
	public float verticalSmoothTime;

	FocusArea focusArea;

	float currentLookAheadX;
	float targetLookAheadX;
	float lookAheadDirX;
	float smoothLookVelocityX;
	float smoothVelcityY;

	bool lookAheadStopped;

	public void SetTarget(GameObject target)
	{
		this.target = target.GetComponent<PlayerController>();
		focusArea = new FocusArea(this.target.bounds, focusAreaSize);
	}

	public void SetTarget(PlayerController target)
	{
		this.target = target.GetComponent<PlayerController>();
		focusArea = new FocusArea(this.target.bounds, focusAreaSize);
	}

	void Start()
	{
		if (!target)
		{
			SetTarget(target);
		}
	}

	void LateUpdate () 
	{
		Debug.Log(focusArea.centre);

		if(!target) return;

		focusArea.Update(target.bounds);

		Vector2 focusPosition = focusArea.centre + Vector2.up * verticalOffset;

		if(focusArea.velocity.x != 0)
		{
			lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
			if(Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0)
			{
				lookAheadStopped = false;
				targetLookAheadX = lookAheadDirX * lookAheadDstX;
			}
			else
			{
				if(!lookAheadStopped)
				{
					lookAheadStopped = true;
					targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX)/ 4f;
				}
				
			}
		}

		currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);
		
		focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelcityY, verticalSmoothTime);
		focusPosition += Vector2.right * currentLookAheadX;
		
		transform.position = (Vector3) focusPosition + Vector3.forward * -15;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = new Color(1,0,0,.5f);
		Gizmos.DrawCube(focusArea.centre, focusAreaSize);
	}

	struct FocusArea
	{
		public Vector2 centre;
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
			centre = new Vector2((left+right)/2 , (top+bottom)/2);
		}

		public void Update(Bounds targetBounds)
		{
			float shiftX = 0;
			if(targetBounds.min.x < left)
			{
				shiftX = targetBounds.min.x - left;
			}
			else if ( targetBounds.max.x > right)
			{
				shiftX = targetBounds.max.x - right;
			}
			
			left += shiftX;
			right += shiftX;
			
			float shiftY = 0;
			if(targetBounds.min.y < bottom)
			{
				shiftY = targetBounds.min.y - bottom;
			}
			else if ( targetBounds.max.y > top)
			{
				shiftY = targetBounds.max.y - top;
			}
			
			bottom += shiftY;
			top += shiftY;
			centre = new Vector2((left+right)/2 , (top+bottom)/2);
			
			velocity = new Vector2(shiftX, shiftY);
		}
	}
}
