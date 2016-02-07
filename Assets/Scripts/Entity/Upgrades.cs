using UnityEngine;
using System.Collections;

public class Upgrades : MonoBehaviour, IUpgradable
{
	private bool hasDoubleJump;

	#region Getters and Setters
	public bool doubleJump
	{
		get
		{
			return hasDoubleJump;
		}

		set
		{
			hasDoubleJump = value;
		}
	}
	#endregion

	// Handle that activation of the Power ups
	public void EnablePowerUp(string powerUp)
	{
		Debug.Log("Enabling the power up: " + powerUp);
		// Check the string to know which power up was enabled
		switch(powerUp)
		{
		case "DoubleJump": doubleJump = true;	break;						// Enables the Double Jump
		}
	}
}
