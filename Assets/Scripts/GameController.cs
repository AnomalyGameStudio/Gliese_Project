using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets._2D;

public class GameController : MonoBehaviour 
{

	public GameObject playerPrefab;												// Player Prefab
	public static GameController instance;										// Singleton from GameController class
	public float gravity = -180;												// Stores the global gravity. This is set by the Player Controller

	Vector3 checkpoint = Vector3.zero;											// Stores the current checkpoint the player is
	Camera2DFollow camera;														// The main camera

	public bool gameOver = false;												// Holds if the game is over
	public Text gameOverText;													// Holds the info for the gameOver

	void Awake()
	{
		// If there isn't a stance of GameController yet
		if(instance == null)
		{
			// Search for the GameController and set it as the instance
			instance = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController> ();
		}

		// Stores 
		camera = Camera.main.GetComponent<Camera2DFollow>();
	}

	void Start()
	{
		if(GameObject.FindGameObjectWithTag("Spawn"))
		{
			checkpoint = GameObject.FindGameObjectWithTag("Spawn").transform.position;
		}
	}

	void Update()
	{
		if(camera.target == null || gameOver)
		{
			if(Input.GetKeyDown(KeyCode.R))
			{
				SpawnPlayer();
			}
		}
	}

	// Sets the Last checkpoint the player has passed
	public void SetCheckpoint(Vector3 checkpoint)
	{
		this.checkpoint = checkpoint;
	}
	
	public void SpawnPlayer()
	{
		gameOverText.text = "";
		gameOver = false;
		Instantiate(playerPrefab, checkpoint, Quaternion.identity);
	}
	
	public void KillPlayer(Transform player)
	{
		gameOverText.text = "You Died!\n Press 'R' to Respawn";
		Destroy(player.gameObject);
	}

	public void GameOver()
	{
		gameOver = true;
		gameOverText.text = "A winner is you!";
	}
}