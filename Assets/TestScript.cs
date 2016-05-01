using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {
	
	public GameObject flipModel;
	
	public float detectionTime;
	float startRun;
	bool firstDetection;
	
	//Movement option
	public float runSpeed;
	public float walkSpeed;
	public bool facingRight = true;
	
	float moveSpeed;
	bool running;
	
	Rigidbody myRB;
	Animator myAnim;
	Transform detectedPlayer;
	
	bool Detected;
	
	// Use this for initialization
	void Start () {
		myRB = GetComponentInParent<Rigidbody>();
		myAnim = GetComponentInParent<Animator>();
		
		running = false;
		Detected = false;
		firstDetection = false;
		moveSpeed = walkSpeed;
		
		if(Random.Range(0,10)>5) Flip();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(Detected)
		{
			if(detectedPlayer.position.x < transform.position.x && facingRight) 
			{
				Flip();
			}
			else if(detectedPlayer.position.x > transform.position.x && !facingRight)
			{
				Flip();
			}
			
			if(!firstDetection)
			{
				startRun = Time.time + detectionTime;
				firstDetection = true;
			}
		}
		if(Detected && !facingRight)
		{
			myRB.velocity = new Vector3((moveSpeed * -1), myRB.velocity.y, 0);
		}
		else if(Detected && facingRight)
		{
			myRB.velocity = new Vector3(moveSpeed, myRB.velocity.y, 0);
		}
		
		if(!running && Detected)
		{
			if(startRun < Time.time)
			{
				moveSpeed = runSpeed;
				myAnim.SetTrigger("run");
				running = true;
			}
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		Debug.Log("Teste");
		if(other.tag == "Player" && !Detected)
		{
			Detected = true;
			detectedPlayer = other.transform;
			myAnim.SetBool("detected", Detected);
			if(detectedPlayer.position.x < 	transform.position.x && facingRight) 
			{
				Flip();
			}
			else if(detectedPlayer.position.x > transform.position.x && !facingRight)
			{
				Flip();
			}
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.tag == "Player")
		{
			firstDetection = false;
			if(running)
			{
				myAnim.SetTrigger("run");
				moveSpeed = walkSpeed;
				running = false;
			}
		}
	}
	
	void Flip()
	{
		facingRight = !facingRight;
		Vector3 theScale = flipModel.transform.localScale;
		theScale.z *= -1;
		flipModel.transform.localScale = theScale;
	}
}
