using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour, IEntity
{
	public float maxHealth;

	private IDamageable<float> enemyStats;

	public IDamageable<float> stats
	{
		get
		{
			if(enemyStats == null)
			{
				enemyStats = new Stats(transform, maxHealth);
			}
			
			return enemyStats;
		}
	}
}
