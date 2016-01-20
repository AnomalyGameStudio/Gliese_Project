using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour 
{
	[System.Serializable]
	public class Stats
	{
		public float currentHealth = 100;
		public float maxHealth = 100;
	}

	public Stats stats;

	public void Damage(float damage)
	{
		stats.currentHealth -= damage;

		if(stats.currentHealth <= 0)
		{
			GameController.instance.KillPlayer(this.transform);
		}
	}
}
