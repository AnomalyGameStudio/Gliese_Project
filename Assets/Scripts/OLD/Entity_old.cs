using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Entity_old : MonoBehaviour
{
	static float currentHealth = 100;
	static float maxHealth = 100;
	public GameObject ragdoll;

	[HideInInspector]
	public Text healthBar;

	public void TakeDamage(float damage)
	{
		currentHealth -= damage;
		currentHealth = Mathf.Clamp(currentHealth, 0, currentHealth);
		if(currentHealth <= 0)
		{
			Die();
		}

		UpdateUI();
	}

	public void Die()
	{
		Debug.Log("Dead");
		Ragdoll r = (Instantiate(ragdoll, transform.position, transform.rotation) as GameObject).GetComponent<Ragdoll>();
		//r.CopyPose(transform);
		Destroy(this.gameObject);
	}

	public static GameObject SpawnEntity(GameObject entityPrefab, Vector3 checkpoint, Text health)
	{
		GameObject currentPlayer = Instantiate(entityPrefab, checkpoint, Quaternion.identity) as GameObject;
		currentPlayer.GetComponent<PlayerControllerImproved>().healthBar = health;
		currentPlayer.GetComponent<PlayerControllerImproved>().ResetHealth();
		currentPlayer.GetComponent<PlayerControllerImproved>().UpdateUI();
		return currentPlayer;
	}

	void ResetHealth()
	{
		currentHealth = maxHealth;
	}

	void UpdateUI()
	{
		healthBar.text = "Health: " + currentHealth + " / " + maxHealth;
	}
}
