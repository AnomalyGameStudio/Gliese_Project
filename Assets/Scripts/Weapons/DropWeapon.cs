using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class DropWeapon : MonoBehaviour, IWeapon
{
	public int weaponSlot;										// The slot the weapon will be
	public int minAmmoDrop;										// The minimum amount of ammo droped
	public int maxAmmoDrop;										// The maximum amount of ammo droped

	int curMaxAmmo;												// The amount of Ammo besides the one in the clip

	public Transform firePoint
	{
		get
		{
			return null;
		}

		set
		{
		}
	}
	
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

	void Awake()
	{
		currentMaxAmmo = (int) Random.Range(minAmmoDrop, maxAmmoDrop);
	}

	void Start()
	{
		Rigidbody rb = GetComponent<Rigidbody> ();
		rb.angularVelocity = Random.insideUnitSphere * 1f;
	}

	public void Shoot(){}
	
	public void SetActive(bool isActive){}
	
	public void Reload(){}
	
	public void AddAmmo(int amount){}

	public void UpdateUI(){}
}
