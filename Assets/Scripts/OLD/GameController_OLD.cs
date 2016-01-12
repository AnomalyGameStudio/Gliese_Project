using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController_OLD : MonoBehaviour 
{
	public static GameController_OLD instance;
	public GameObject playerPrefab;
	public Text health; //TODO Provisorio

	public float gravity;

	Vector3 checkpoint = Vector3.zero;
	GameObject currentPlayer;
	CameraControl_OLD camera;

	public static int levelCount = 2;
	public static int currentLevel = 1;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		camera = Camera.main.GetComponent<CameraControl_OLD> ();
		
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
		currentPlayer = Entity_old.SpawnEntity(playerPrefab, checkpoint, health);
		camera.SetTarget(currentPlayer);
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
