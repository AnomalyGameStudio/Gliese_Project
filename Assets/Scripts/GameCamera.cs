using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour 
{
	private float trackSpeed = 10;
	private Transform target;

	public void SetTarget(Transform t)
	{
		target = t;
	}

	void LateUpdate()
	{
		if(target)
		{
			float x = IncrementTowards(target.position.x, target.position.x, trackSpeed);
			float y = IncrementTowards(target.position.y, target.position.y, trackSpeed);
			transform.position = new Vector3(x, y, transform.position.z);
		}
	}

	private float IncrementTowards(float n, float target, float a)
	{
		if(n == target)
		{
			return n;
		}
		else
		{
			float dir = Mathf.Sign(target - n); //Must N be increased or decreased to get closer to the target
			n += a * Time.deltaTime * dir;
			return (dir == Mathf.Sign(target-n)) ? n : target; //If N has passed target then return target, otherwise return N.
		}
	}
}
