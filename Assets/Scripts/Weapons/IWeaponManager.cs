using UnityEngine;
using System.Collections;

public interface IWeaponManager 
{
	IWeapon currentWeapon
	{
		get;
		set;
	}

	void ChangeWeapon(int weaponNumber);

	void AddWeapon(Transform weapon);
}
