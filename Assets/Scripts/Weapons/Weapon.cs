using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour, IWeapon
{
	public GameObject bullet;									// Prefab of the bullet
	public float fireRate;										// The fireRate of the weapon. 0 is single fire, more than 0 should be automatic. //TODO Test with 0
	public float reloadTime;									// Time needed to reaload
	public int weaponSlot;										// The slot the weapon will be
	public int clipSize;										// The size of the clip of the weapon
	public int maxAmmo;											// The maximum amount of Ammo the weapon can have

	float timeToFire = 0;										// The time when the weapon can fire next
	int currentClipAmmo;										// The amount of Ammo currently on the clip
	int curMaxAmmo;												// The amount of Ammo besides the one in the clip

	private Transform weaponFirePoint;							// The muzzle of the weapon
	private bool active;										// Tells if the weapon is active or not

	#region Setters and Getters
	// Getter and Setter for the firepoint
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

	// Getter for the slot
	public int slot
	{
		get
		{
			return weaponSlot;
		}
	}

	// Getter and setter for the MaxAmmo
	public int currentMaxAmmo
	{
		get
		{
			return curMaxAmmo;
		}
		set
		{
			curMaxAmmo = value;
		}
	}
	#endregion

	void Awake()
	{
		// Looks for the firePoint. It has to be a child of the weapon
		firePoint = transform.FindChild("FirePoint");

		// If the firepoint is not found raise an error
		if(firePoint == null)
		{
			Debug.LogError("Firepoint object not found.");
		}
	}

	// TODO Move to the player input
	void Update()
	{
		if(Input.GetButton("Fire1"))
		{
			Shoot();
		}

		if(Input.GetButton("Reload"))
		{
			Reload();
		}
	}

	public void Shoot()
	{
		// Check if it is time to fire and if the weapon is active
		if(Time.time < timeToFire || !active)
		{
			return;
		}

		// Check if there is ammo in the clip
		if(currentClipAmmo > 0)
		{
			// Set the next time to fire
			timeToFire = Time.time + 1/fireRate;
			// Instantiate the bullet prefab in the muzzle of the weapon
			Instantiate (bullet, firePoint.position, firePoint.rotation);
			// Remove one bullet from the clip
			currentClipAmmo--;

			UpdateUI();

			if(currentClipAmmo == 0) Reload();
		}
		else
		{
			// In case there is no ammo in the clip, Reload
			Reload();
		}

	}

	public void SetActive(bool isActive)
	{
		// Set if active or not
		this.active = isActive;
		// Change the Active property of the object according to what was passed to the method
		gameObject.SetActive(isActive);

		if(active)
		{
			UpdateUI();
		}
	}

	public void Reload()
	{
		// The amount of ammo needed to fill the clip to the max
		int amountToReload = clipSize - currentClipAmmo;
		// Add to the clip the max it can. The lesser of the amount needed or the total ammo
		currentClipAmmo += Mathf.Min(amountToReload, currentMaxAmmo);
		// Update the ammo amount
		AddAmmo(-amountToReload);
		// Hold the fire with the time needed to reaload
		timeToFire = Time.time + reloadTime;

		Invoke("UpdateUI", reloadTime);
	}

	public void AddAmmo(int amount)
	{
		// Add the amount to the current Ammo
		currentMaxAmmo += amount;
		// Makes sure the currentMaxAmmo is never negative neither greater than the Max ammount
		currentMaxAmmo = Mathf.Clamp(currentMaxAmmo, 0, maxAmmo);

		// In case there is no Ammo on the clip and we added Ammo, do an auto-reload
		if(currentClipAmmo == 0 && currentMaxAmmo > 0)
		{
			Reload();
		}

		Invoke("UpdateUI", reloadTime);
	}

	public void UpdateUI()
	{
		Debug.Log("executed the update at: " + Time.time);
		if(active)
		{
			GameController.instance.UpdateAmmo(currentClipAmmo, currentMaxAmmo);
		}
	}
}
