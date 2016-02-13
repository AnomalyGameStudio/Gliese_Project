using UnityEngine;
using System.Collections;

public class KillBox : MonoBehaviour 
{
	void OnTriggerEnter(Collider c)
	{
		if(c.tag == "Player")
		{
			GameController.instance.KillPlayer(c.transform);
			//c.GetComponent<Entity_old>().TakeDamage(damage);			                                  
		}
	}
}