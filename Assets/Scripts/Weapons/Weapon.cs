using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour, IWeapon
{
	//public LayerMask toHit;
	public GameObject bullet;
	public float fireRate;
	float timeToFire = 0;

	private Transform weaponFirePoint;

	public Transform firePoint
	{
		get
		{
			return weaponFirePoint;
		}

		set
		{
			weaponFirePoint = value;
		}
	}

	void Awake()
	{
		firePoint = transform.FindChild("FirePoint");

		if(firePoint == null)
		{
			Debug.LogError("Firepoint object not found.");
		}
	}

	void Update()
	{
		if(Input.GetButton("Fire1"))
		{
			Shoot();
		}
	}

	public void Shoot()
	{
		if(Time.time < timeToFire)
		{
			return;
		}
		timeToFire = Time.time + 1/fireRate;
		Instantiate (bullet, firePoint.position, firePoint.rotation);
	}
	
}
