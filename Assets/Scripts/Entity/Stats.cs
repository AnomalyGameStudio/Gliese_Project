using UnityEngine;
using System.Collections;

public class Stats : IDamageable<float>
{
	private Transform entityTransform;
	private float entityCurrentHealth;
	private float entityMaxHealth;

	#region Getters and Setters
	public Transform entity
	{
		get
		{
			return entityTransform;
		}
		set
		{
			entityTransform = value;
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

	// Constructor - Guarantees that we will have the value of entity
	public Stats(Transform entity, float maxHealth)
	{
		this.entity = entity;
		this.maxHealth = maxHealth;
		this.currentHealth = maxHealth;
	}

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
