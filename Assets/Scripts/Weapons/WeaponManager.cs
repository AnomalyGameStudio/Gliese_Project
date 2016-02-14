using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour , IWeaponManager
{
	public Transform weaponBone;																// Holds the reference to the weapon bone

	static Dictionary<int, IWeapon> weapons = new Dictionary<int, IWeapon>();					// Static reference to the equiped weapons
	static int curWeaponSlot;																	// Static reference to the current equiped weapon

	Dictionary<int, IWeapon> allWeapons = new Dictionary<int, IWeapon>();						// Reference to all weapons child of the weapon bone
	Animator animator;																			// Reference to the animator component
	IWeapon curWeapon;																			// Reference to the equiped weapon

	public IWeapon currentWeapon
	{
		get
		{
			return curWeapon;
		}
		set
		{
			// When seting the current weapon, updates the slot too
			curWeapon = value;
			curWeaponSlot = value.slot;
		}
	}

	void Start()
	{
		// Gets the animator component
		animator = GetComponent<Animator>();

		// checks if there is a weapon Bone on the player
		if(weaponBone == null)
		{
			// TODO Don't know if it is the best way
			Debug.LogError("Weapon bone not found");
		}

		// Checks in each child of the weaponBone for weapons
		for(int i = 0; i < weaponBone.childCount; i++)
		{
			// Gets the child
			Transform child = weaponBone.GetChild(i);

			// Try to get the IWeapon component from the child
			IWeapon weapon = child.GetComponent<IWeapon>();
			// If there is no weapon component
			if(weapon != null)
			{
				IWeapon equipedWeapon;
				// Adds the reference to the weapon on the list with all weapons
				allWeapons.Add(weapon.slot, weapon);

				// Looks if the player already have the weapon equiped
				if(weapons.TryGetValue(weapon.slot, out equipedWeapon))
				{
					// Replaces the old reference to the new one
					weapons.Remove(weapon.slot);
					weapons.Add(weapon.slot, weapon);
				}
			}
		}

		// If there is a weapon equips
		if(curWeaponSlot != 0)
		{
			// Change to the equiped weapon
			ChangeWeapon(curWeaponSlot);
		}
	}

	public void ChangeWeapon(int weaponNumber)
	{
		IWeapon selectedWeapon;														// Holds the reference to the chosen weapon

		// Try to find the selected slot
		if(weapons.TryGetValue(weaponNumber, out selectedWeapon))
		{
			// If there is a equiped weapon
			if(currentWeapon != null)
			{
				// Disable it
				currentWeapon.SetActive(false);
			}

			// Activate the chosen weapon
			weapons[weaponNumber].SetActive(true);
			currentWeapon = weapons[weaponNumber];

			// Updates the animator
			animator.SetFloat("Weapon", weaponNumber);
		}
	}
	
	public void AddWeapon(Transform weapon)
	{
		IWeapon weaponFound;														// Holds the reference for the weapon the player already has
		IWeapon weaponReference;													// Holds the reference for all weapons
		IWeapon pickedWeapon = weapon.GetComponent<IWeapon>();						// The reference to the weapon the player just picked

		// If It is not a weapon do nothing
		if(pickedWeapon == null)
		{
			return;
		}

		// Looks for the found weapon on the reference to all weapons
		if(allWeapons.TryGetValue(pickedWeapon.slot, out weaponReference))
		{
			// Looks if the player already have this weapon
			if(!weapons.TryGetValue(pickedWeapon.slot, out weaponFound))
			{
				// If not found it yet, add to the list
				weapons.Add(pickedWeapon.slot, weaponReference);
			}

			// Add the ammo to the weapon just picked
			weaponReference.AddAmmo(pickedWeapon.currentMaxAmmo);
		}

		// If there is no weapon equiped yet
		if(currentWeapon == null)
		{
			// Equips the weapon found
			currentWeapon = weaponReference;
			currentWeapon.SetActive(true);
		}
		else
		{
			// Check if the weapon found is already equiped
			if(currentWeapon.slot != weaponReference.slot)
			{
				// If not equiped, makes sure it is not active
				weaponReference.SetActive(false);
			}
		}
	}
}
