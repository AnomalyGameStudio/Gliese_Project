using UnityEngine;
using System.Collections;

public class Spike : MonoBehaviour 
{
	void OnTriggerEnter(Collider c)
	{
		if(c.tag == "Player")
		{
			c.GetComponent<Entity_old>().Die();			                                  
		}
	}
}
