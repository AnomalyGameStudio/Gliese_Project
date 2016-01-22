using UnityEngine;
using System.Collections;

public class KillBox : MonoBehaviour 
{
	void Awake()
	{
		Debug.Log("KillBox: Active");
	}

	void OnTriggerEnter(Collider c)
	{
		Debug.Log("!");
		if(c.tag == "Player")
		{
			Debug.Log("?");
			GameController.instance.KillPlayer(c.transform);
			//c.GetComponent<Entity_old>().TakeDamage(damage);			                                  
		}
	}
}
