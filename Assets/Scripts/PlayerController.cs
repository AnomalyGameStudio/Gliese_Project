using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterController))]
[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(PlayerControllPhysics))]
public class PlayerController : MonoBehaviour 
{
	[HideInInspector] public CharacterController controller;			// Reference to the CharacterController component
	[HideInInspector] public Vector2 playerInput;						// The player's input to be used in the camera
	[HideInInspector] public Bounds bounds;								// Variable used to provide the bounds to the camera TODO: Check whether it is used

	public Vector2 wallJumpOff;											// The velocity of the jump off the wall
	public Vector2 wallLeap;											// The velocity of a wall leap

	public float moveSpeed = 12f;										// Control the speed of the player
	//public float jumpSpeed = 8f;										// Control the jumpSpeed. TODO Remove this part. Deprecated

	public float maxJumpHeight = 4f;									// The maximum height of a Jump
	public float minJumpHeight = 1f;									// The minimum Height of a Jump
	public float timeToJumpApex = 0.4f;									// The time to achieve max jump height

	public float wallSlideSpeedMax = 3;
	public float wallStickTime = .25f;
	float timeToWallUnstick = .5f;

	Animator animator;													// Reference to the Animator component
	Vector3 velocity = Vector3.zero;									// Stores the movement vector of the player

	float accelerationTimeAirborne = .2f;								// Controls the acceleration when the player is in the Air
	float accelerationTimeGrounded = .1f;								// Controls the acceleration when the player is on the Ground.
	float velocityXSmoothing;											// Variable to hold the SmoothDamp of the X velocity
	float maxJumpVelocity;												// The velocity to achieve Max Jump Height
	float minJumpVelocity;												// The velocity to achieve Min Jump Height
	float gravity;														// The gravity

	float xScale;														// Holds the value of the scale in the X axys

	// TODO Temp - Arrumar uma maneira melhor
	PlayerControllPhysics playerPhysics;

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

		// Stores the scale of the player
		xScale = transform.localScale.x;

		// TEMP - To remove
		playerPhysics = GetComponent<PlayerControllPhysics>();
	}

	void Start()
	{
		// Calculate the gravity on the player. TODO Move this calculation to a GameController
		gravity = - (2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		Debug.Log(gravity);
		// Calculate the velocity to achieve max Jump Height
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

		// Calculate the velocity to achieve min Jump Height
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
	}

	void Update()
	{
		bool wallSliding = false;
		int wallDirX = (playerPhysics.collisions.left) ? -1 : 1;

		// Set the Attribute Jumping of the animator based on the player position
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

		// Check if player is colliding with something and is not on ground
		if((playerPhysics.collisions.left || playerPhysics.collisions.right) && !controller.isGrounded && velocity.y < 0)
		{
			// Set the wall sliding to true
			wallSliding = true;

			// Check if the current Y velocity is lesser than the wallSlideMaxSpeed
			if(velocity.y < -wallSlideSpeedMax)
			{
				velocity.y = -wallSlideSpeedMax;
			}

			// Check if reached the time to unstick the wall
			if(timeToWallUnstick > 0)
			{
				velocityXSmoothing = 0;
				velocity.x = 0;
				
				if(playerInput.x != wallDirX && playerInput.x != 0)
				{
					timeToWallUnstick -= Time.deltaTime;
				}
				else
				{
					timeToWallUnstick = wallStickTime;
				}
			}
			else
			{
				timeToWallUnstick = wallStickTime;
			}
		}

		// Starts the Jumping Sequence
		if(Input.GetButtonDown("Jump"))
		{
			// Check if the player is sliding on the Wall
			if(wallSliding)
			{
				// If the player is inputing nothing or moving to the same side as the wall it will do a Jump Off
				if (playerInput.x == 0 || wallDirX == playerInput.x)
				{
					velocity.x = -wallDirX * wallJumpOff.x;
					velocity.y = wallJumpOff.y;
				}
				else // If the player is moving to the oposite side will do a leap
				{
					velocity.x = -wallDirX * wallLeap.x;
					velocity.y = wallLeap.y;
				}
			}
			// If it is on the ground it will Jump
			if(controller.isGrounded)
			{
				velocity.y = maxJumpVelocity;
			}
		}
		else
		{
			if(controller.isGrounded)
			{
				velocity.y = 0;
			}
		}

		// Check if the player already released the jump button
		if(Input.GetButtonUp("Jump"))
		{
			// Check if the Y velocity is greater than the minimum after releasing the jump button
			if(velocity.y > minJumpVelocity)
			{
				// Set the Y velocity to the minimum Jump velocity
				velocity.y = minJumpVelocity;
			}
		}

		// Check if the player is inputing any X velocity and flips the model to the correct side
		if(playerInput.x != 0 && !wallSliding)
		{
			Flip(directionX);
		}

		// Pass to the animator the current velocity of the player
		animator.SetFloat("Speed", Mathf.Abs(velocity.x));

		// TEMP
		playerPhysics.CheckCollider(ref velocity);

		velocity.y += gravity * Time.deltaTime;
		controller.Move(velocity * Time.deltaTime);
	}

	void Flip(float dirX)
	{
		Vector3 scale = transform.localScale;
		scale.x = dirX * xScale;

		transform.localScale = scale;
	}

	void OnTriggerEnter(Collider collider)
	{
		if(collider.tag == "Checkpoint")
		{
			GameController.instance.SetCheckpoint(collider.transform.position);
		}
	}
}
