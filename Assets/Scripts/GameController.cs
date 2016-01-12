using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour 
{
	// Singleton from GameController class
	public static GameController instance;

	// Stores the current checkpoint the player is
	Vector3 checkpoint = Vector3.zero;

	void Start()
	{
		// If there isn't a stance of GameController yet
		if(instance == null)
		{
			// Search for the GameController and set it as the instance
			instance = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController> ();
		}
	}
	
	public void SetCheckpoint(Vector3 checkpoint)
	{
		this.checkpoint = checkpoint;
	}

	public void SpawnPlayer()
	{

	}

	public void KillPlayer()
	{

	}
}