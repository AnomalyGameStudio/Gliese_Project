using UnityEngine;
using System.Collections;

public interface IDamageable <T>
{
	Transform entity
	{
		get;
		set;
	}

	T currentHealth
	{
		get;
		set;
	}

	T maxHealth
	{
		get;
		set;
	}

	void Damage(T damage);
}
