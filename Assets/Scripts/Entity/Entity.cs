using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour, IEntity
{
	public float maxHealth;

	public static IUpgradable upgrades = new Upgrades();

	private IDamageable<float> playerStats;

	public IDamageable<float> stats 
	{
		get
		{
			if(playerStats == null)
			{
				playerStats = new Stats(transform, maxHealth);
			}
			
			return playerStats;
		}
	}
}
