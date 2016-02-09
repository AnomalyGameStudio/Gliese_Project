using UnityEngine;
using System.Collections;

public class DestroyMe : MonoBehaviour 
{
	public float aliveTime;

	void Awake()
	{
		Destroy(gameObject	, aliveTime);
	}
}
