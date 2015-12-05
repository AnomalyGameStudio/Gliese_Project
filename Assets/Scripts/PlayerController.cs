using UnityEngine;
using System.Collections;

[RequireComponent (typeof(PlayerPhysics))]
public class PlayerController : MonoBehaviour 
{
	//Player Handling
	public float walkSpeed = 8;
	public float runSpeed = 12;
	public float acceleration = 30;
	public float gravity = 20;
	public float jumpHeight = 12;
	public float slideDecelaration = 10;

	private float animationSpeed;
	private float currentSpeed;
	private float targetSpeed;
	private Vector2 amountToMove;

	//states
	private bool jumping;
	private bool sliding;


	private PlayerPhysics playerPhysics;
	private Animator animator;

	void Start()
	{
		playerPhysics = GetComponent<PlayerPhysics> ();
		animator = GetComponent<Animator> ();

		animator.SetLayerWeight(1, 1);
	}

	void Update()
	{
		//Reset Accelaration uppon colision
		if(playerPhysics.movementStopped)
		{
			targetSpeed = 0;
			currentSpeed = 0;
		}

		//If player is touching the ground
		if(playerPhysics.grounded)
		{
			amountToMove.y = 0;

			if(jumping)
			{
				jumping = false;
				animator.SetBool("Jumping", false);
			}

			if(sliding)
			{
				if(Mathf.Abs(currentSpeed) < .25f)
				{
					sliding = false;
					animator.SetBool("Sliding", false);
					playerPhysics.ResetCollider();
				}
			}

			//Jump
			if (Input.GetButtonDown("Jump"))
			{
				amountToMove.y = jumpHeight;
				jumping = true;
				animator.SetBool("Jumping", true);
			}

			//Slide Input
			if(Input.GetButtonDown("Slide"))
			{
				sliding = true;
				animator.SetBool("Sliding", true);
				targetSpeed = 0;

				playerPhysics.SetCollider(new Vector3(10.3f, 1.5f, 3), new Vector3(.35f, .75f, 0));
			}
		}

		animationSpeed = IncrementTowards(animationSpeed, Mathf.Abs(targetSpeed), acceleration);
		animator.SetFloat("Speed", animationSpeed);


		if(!sliding)
		{
			//Input
			float speed = (Input.GetButton("Run")) ? runSpeed : walkSpeed; //TODO Bug, aumenta velocidade no ar
			targetSpeed = Input.GetAxisRaw("Horizontal") * speed;
			currentSpeed = IncrementTowards(currentSpeed, targetSpeed, acceleration);

			//Face direction
			float moveDir = Input.GetAxisRaw("Horizontal");
			if (moveDir != 0)
			{
				transform.eulerAngles = (moveDir > 0) ? Vector3.up * 180 : Vector3.zero;
			}
		}
		else
		{
			currentSpeed = IncrementTowards(currentSpeed, targetSpeed, slideDecelaration);
		}

		//Set amount to move
		amountToMove.x = currentSpeed;
		amountToMove.y -= gravity * Time.deltaTime;
		playerPhysics.Move(amountToMove * Time.deltaTime);
	}

	private float IncrementTowards(float n, float target, float a)
	{
		if(n == target)	
		{
			return n;
		}
		else
		{
			float dir = Mathf.Sign(target - n); //Must N be increased or decreased to get closer to the target
			n += a * Time.deltaTime * dir;
			return (dir == Mathf.Sign(target-n)) ? n : target; //If N has passed target then return target, otherwise return N.
		}
	}
}
