using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour 
{
	// Class used to hold the player info
	[System.Serializable]
	public class Stats
	{	
		#region player info
		public float currentHealth = 100;										// Player's current health
		public float maxHealth = 100;											// Player's max health
		#endregion

		#region upgrades info
		public bool doubleJump = false;											// Flags if the player has the ability to double Jump
		#endregion
	}

	public Stats stats;

	// Handles the damage taken by the player
	public void Damage(float damage)
	{
		// Do the damage
		stats.currentHealth -= damage;

		// If the damage is 0 or bellow kills the player
		if(stats.currentHealth <= 0)
		{
			GameController.instance.KillPlayer(this.transform);
		}
	}

	// Handle that activation of the Power ups
	public void EnablePowerUp(string powerUp)
	{
		Debug.Log("Enabling the power up: " + powerUp);
		// Check the string to know which power up was enabled
		switch(powerUp)
		{
		case "DoubleJump": stats.doubleJump = true;	break;						// Enables the Double Jump
		}
	}
}
