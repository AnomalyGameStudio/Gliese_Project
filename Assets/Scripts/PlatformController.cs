using UnityEngine;
using System.Collections;

public class PlatformController : MonoBehaviour 
{
	public float speed = 2f;

	void Update()
	{
		transform.Translate(speed * Vector3.right * Time.deltaTime);
	}
}
