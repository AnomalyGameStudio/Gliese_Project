using UnityEngine;
using System.Collections;

/*
 * Creator: Eduardo Luiz Lopes
 * Description: This script is reponsible to move the projectiles
*/

[RequireComponent (typeof(Rigidbody))]
public class Bullet : MonoBehaviour 
{
	public float speed = 20f;								// The speed of the projectile
	public float timeAlive = 5f;							// The time the bullet will remain alive
	public float damage = 10;								// The damage of each bullet
	void Start()
	{
		// Gets the component and adds the speed to the RigidBody object
		Rigidbody rb = GetComponent<Rigidbody>();
		rb.velocity = transform.right * speed;

		Destroy(gameObject, timeAlive);
	}

	void OnTriggerEnter(Collider c)
	{

		Destroy(gameObject);

		if(c.tag ==	 "Enemy")
		{
			IEntity enemy = c.GetComponent<Enemy>();

			if(enemy != null)
			{
				enemy.stats.Damage(damage);
			}

			Debug.Log("Enemy Hit");
		}
	}
}
