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

	int currentMaxAmmo
	{
		get;
	}

	void Shoot();

	void SetActive(bool isActive);

	void Reload();
	//IEnumerator Reload();

	void AddAmmo(int amount);

	void UpdateUI();
}