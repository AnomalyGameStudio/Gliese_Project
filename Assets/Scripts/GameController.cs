using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class GameController : MonoBehaviour 
{
	// Player Prefab
	public GameObject playerPrefab;

	// Singleton from GameController class
	public static GameController instance;
	
	// Stores the global gravity. This is set by the Player Controller
	public float gravity = -180;
	
	// Stores the current checkpoint the player is
	Vector3 checkpoint = Vector3.zero;

	// The main camera
	Camera2DFollow camera;
	
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
		if(camera.target == null && Input.GetKeyDown(KeyCode.R))
		{
			SpawnPlayer();
		}
	}

	// Sets the Last checkpoint the player has passed
	public void SetCheckpoint(Vector3 checkpoint)
	{
		this.checkpoint = checkpoint;
	}
	
	public void SpawnPlayer()
	{
			Instantiate(playerPrefab, checkpoint, Quaternion.identity);
	}
	
	public void KillPlayer(Transform player)
	{
		Destroy(player.gameObject);
	}
}