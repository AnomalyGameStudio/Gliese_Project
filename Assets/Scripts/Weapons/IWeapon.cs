using UnityEngine;
using System.Collections;

public interface IWeapon
{
	Transform firePoint
	{
		get;
		set;
	}

	void Shoot();
}
