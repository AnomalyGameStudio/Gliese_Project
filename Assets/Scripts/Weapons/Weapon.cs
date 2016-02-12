using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour, IWeapon
{
	//public LayerMask toHit;
	public GameObject bullet;
	public float fireRate;
	public int weaponSlot;

	float timeToFire = 0;

	private Transform weaponFirePoint;
	private bool active;


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

	public int slot
	{
		get
		{
			return weaponSlot;
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
		if(Time.time < timeToFire || !active)
		{
			return;
		}
		timeToFire = Time.time + 1/fireRate;

		Instantiate (bullet, firePoint.position, firePoint.rotation);
	}

	public void setActive(bool isActive)
	{
		this.active = isActive;
		gameObject.SetActive(isActive);
	}

	public void Reload()
	{

	}
	
	public void AddAmmo(float amount)
	{

	}
}
