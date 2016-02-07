using UnityEngine;
using System.Collections;

public class Stats : IDamageable<float>
{
	private float entityCurrentHealth;
	private float entityMaxHealth;

	#region Getters and Setters
	public Transform entity
	{
		get
		{
			return entity;
		}
		set
		{
			entity = value;
		}
	}

	public float currentHealth
	{
		get
		{
			return entityCurrentHealth;
		}
		set
		{
			entityCurrentHealth = value;
		}
	}

	public float maxHealth
	{
		get
		{
			return entityMaxHealth;
		}
		set
		{
			entityMaxHealth = value;
		}
	}
	#endregion

	// Handles the damage taken by the player
	public void Damage(float damage)
	{
		//	 Do the damage
		currentHealth -= damage;
		
		// If the damage is 0 or bellow kills the player
		if(currentHealth <= 0)
		{
			GameController.instance.KillPlayer(entity);
		}
	}
}
