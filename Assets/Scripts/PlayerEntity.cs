using UnityEngine;
using System.Collections;

public class PlayerEntity : Entity
{
	void Awake()
	{
		//stats = new Stats1();
	}

	public void Damage(float damage)
	{
		stats.currentHealth -= damage;
		if(stats.currentHealth <= 0)
		{
			Debug.Log("Dead");
		}
	}
}
