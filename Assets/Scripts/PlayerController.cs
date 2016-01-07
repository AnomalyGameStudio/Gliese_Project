using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterController))]
[RequireComponent (typeof(Animator))]
public class PlayerController : MonoBehaviour 
{
	[HideInInspector] public CharacterController controller;			// Reference to the CharacterController component
	[HideInInspector] public Vector2 playerInput;						// The player's input to be used in the camera
	[HideInInspector] public Bounds bounds;								// Variable used to provide the bounds to the camera TODO: Check whether it is used

	public float moveSpeed = 12f;										// Control the speed of the player
	public float jumpSpeed = 8f;										// Control the jumpSpeed. TODO Remove this part. Deprecated

	public float maxJumpHeight = 4f;									// The maximum height of a Jump
	public float minJumpHeight = 1f;									// The minimum Height of a Jump
	public float timeToJumpApex = 0.4f;									// The time to achieve max jump height
	
	Animator animator;													// Reference to the Animator component
	Vector3 velocity = Vector3.zero;									// Stores the movement vector of the player

	float accelerationTimeAirborne = .2f;								// Controls the acceleration when the player is in the Air
	float accelerationTimeGrounded = .1f;								// Controls the acceleration when the player is on the Ground.
	float velocityXSmoothing;											// Variable to hold the SmoothDamp of the X velocity
	float maxJumpVelocity;												// The velocity to achieve Max Jump Height
	float minJumpVelocity;												// The velocity to achieve Min Jump Height
	float gravity;														// The gravity


	void Awake()
	{
		// Gets the Character Controller component
		controller = GetComponent<CharacterController>();
		
		// Gets the animator component
		animator = GetComponent<Animator>();
		
		// Check if the Character Controller was found
		if(controller == null)
		{
			Debug.LogError("CharacterController component not found.");
		}
		
		// Check if the animator component was found
		if(animator == null)
		{
			Debug.LogError("Animator component not found");
		}
	}

	void Start()
	{
		// Calculate the gravity on the player. TODO Move this calculation to a GameController
		gravity = - (2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);

		// Calculate the velocity to achieve max Jump Height
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

		// Calculate the velocity to achieve min Jump Height
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
	}

	void Update()
	{
		animator.SetBool("Jumping", !controller.isGrounded);

		// Stores the input of the player
		playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		// Stores the direction the player is facing
		float directionX = Mathf.Sign(playerInput.x);

		// Assigns the acceleration according to whether player is on the ground or not.
		float currentAcceleration = (controller.isGrounded) ? accelerationTimeGrounded : accelerationTimeAirborne;

		// Calculate the target Velocity
		float targetVelocityX = playerInput.x * moveSpeed;

		// Calculate the X velocity
		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, currentAcceleration);

		if (controller.isGrounded) 
		{
			//animator.SetBool("Jumping", false);
			/*
			if (Input.GetButton("Jump"))
			{
				velocity.y = jumpSpeed;
				animator.SetBool("Jumping", true);
			}
			*/
		}

		if(Input.GetButtonDown("Jump"))
		{
			if(controller.isGrounded)
			{
				//animator.SetBool("Jumping", true);
				velocity.y = maxJumpVelocity;
			}
		}
		if(Input.GetButtonUp("Jump"))
		{
			if(velocity.y > minJumpVelocity)
			{
				velocity.y = minJumpVelocity;
			}
		}

		if(playerInput.x != 0)
		{
			Flip(directionX);
		}

		animator.SetFloat("Speed", Mathf.Abs(velocity.x));

		velocity.y += gravity * Time.deltaTime;
		controller.Move(velocity * Time.deltaTime);
	}

	void Flip(float dirX)
	{
		Vector3 scale = transform.localScale;
		scale.x = dirX * (-1);

		transform.localScale = scale;
	}
}
