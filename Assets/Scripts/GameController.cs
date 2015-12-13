using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour 
{
	public static GameController instance;
	public GameObject PlayerPrefab;
	public float gravity;
	Vector3 checkpoint = Vector3.zero;
	GameObject currentPlayer;
	CameraControl camera;

	public static int levelCount = 2;
	public static int currentLevel = 1;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		camera = Camera.main.GetComponent<CameraControl> ();
		
		if(GameObject.FindGameObjectWithTag("Spawn"))
		{
			checkpoint = GameObject.FindGameObjectWithTag("Spawn").transform.position;
		}
		
		SpawnPlayer();
	}

	void Update()
	{
		if(!currentPlayer)
		{
			if(Input.GetButtonDown("Respawn"))
			{
				SpawnPlayer();
			}
		}
	}

	void SpawnPlayer()
	{
		currentPlayer = Instantiate(PlayerPrefab, checkpoint, Quaternion.identity) as GameObject;
		camera.SetTarget(currentPlayer	);
	}

	public void SetCheckpoint(Vector3 checkpoint)
	{
		this.checkpoint = checkpoint;
	}

	public void EndLevel()
	{
		if(currentLevel < levelCount)
		{
			currentLevel++;
			Application.LoadLevel("Level " + currentLevel);
		}
		else
		{
			Debug.Log("Game Over");
		}
	}
}
