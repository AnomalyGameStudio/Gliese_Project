using UnityEngine;
using System.Collections;

public interface IWeapon
{
	Transform firePoint
	{
		get;
		set;
	}

	int slot
	{
		get;
	}

	void Shoot();

	void setActive(bool isActive);

	void Reload();

	void AddAmmo(int amount);
}