using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour 
{
	public IDamageable<float> stats = new Stats();
	public static IUpgradable upgrades = new Upgrades();

	void OnAwake()
	{
		stats.entity = transform;
	}
}
