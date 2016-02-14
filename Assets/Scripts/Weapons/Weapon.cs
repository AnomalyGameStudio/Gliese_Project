using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour, IWeapon
{
	//public LayerMask toHit;
	public GameObject bullet;
	public float fireRate;
	public int weaponSlot;
	public int clipSize;
	public int maxAmmo;
	public int currentClipAmmo;
	public int currentMaxAmmo;
	public float reloadTime;

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

		// TODO Create a Key to reload
		if(Input.GetKeyDown(KeyCode.T))
		{
			Reload();
		}
	}

	public void Shoot()
	{
		if(Time.time < timeToFire || !active)
		{
			return;
		}

		if(currentClipAmmo > 0)
		{
			timeToFire = Time.time + 1/fireRate;

			Instantiate (bullet, firePoint.position, firePoint.rotation);
			currentClipAmmo--;
		}
		else
		{
			Reload();
		}
	}

	public void setActive(bool isActive)
	{
		this.active = isActive;
		gameObject.SetActive(isActive);
	}
	
	public void Reload()
	{
		int amountToReload = clipSize - currentClipAmmo;
		currentClipAmmo += Mathf.Min(amountToReload, currentMaxAmmo);
		AddAmmo(-amountToReload);
		timeToFire = Time.time + reloadTime;
	}
	
	public void AddAmmo(int amount)
	{
		currentMaxAmmo += amount;
		currentMaxAmmo = Mathf.Clamp(currentMaxAmmo, 0, maxAmmo);

		if(currentClipAmmo == 0 && currentMaxAmmo > 0)
		{
			Reload();
		}
	}
}
