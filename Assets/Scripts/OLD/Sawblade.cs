using UnityEngine;
using System.Collections;

public class Sawblade : MonoBehaviour 
{
	public float speed = 300;
	public float damage;

	void Update()
	{
		transform.Rotate(Vector3.forward * speed * Time.deltaTime, Space.World);
	}

	void OnTriggerEnter(Collider c)
	{
		if(c.tag == "Player")
		{
			c.GetComponent<Entity_old>().TakeDamage(damage);			                                  
		}
	}
}
