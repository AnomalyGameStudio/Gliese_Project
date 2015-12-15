using UnityEngine;
using System.Collections;

public class MovementController : MonoBehaviour 
{
	public struct CollisionInfo
	{
		public bool left, right;
		public bool above, bellow;

		public void reset()
		{
			left = right = false;
			above = bellow = false;
		}
	}

	public void Move(ref Vector3 velocity)
	{

	}

	void HorizontalCollisions(ref Vector3 velocity)
	{

	}
}
