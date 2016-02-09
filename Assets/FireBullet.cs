using UnityEngine;
using System.Collections;

public class FireBullet : MonoBehaviour 
{
	public float timeBetweenBullets = 0.15f;

	public GameObject projectile;

	float nextBullet;

	void Awake()
	{
		nextBullet = 0f;
	}

	void Update()
	{
		if(Input.GetAxisRaw("Fire1") > 0 && nextBullet < Time.time)
		{
			nextBullet = Time.time + timeBetweenBullets;

			Vector3 rot = new Vector3(0f, 90f, 0f);

			Instantiate(projectile, transform.position, Quaternion.Euler(rot));
		}
	}
}
