using UnityEngine;
using System.Collections;

[RequireComponent (typeof(PlayerPhysicsImproved))]
public class PlayerControllerImproved : Entity 
{
	public Vector2 wallJumpOff;											// The velocity of the jump off the wall
	public Vector2 wallLeap;											// The velocity of a wall leap
	public Vector2 doubleJumpVelocity;									// The velocity of the Double Jump

	public float moveSpeed = 12f;										// Control the speed of the player
	public float maxJumpHeight = 4f;									// The maximum height of a Jump
	public float minJumpHeight = 1f;									// The minimum Height of a Jump
	public float timeToJumpApex = 0.4f;									// The time to achieve max jump height
	public float wallSlideSpeedMax = 3;									// The speed of will slide down the wall
	public float wallStickTime = .25f;									// The time the player will be stuck to the wall while hold the oposing direction

	GameController gameController;										// Holds the instance of the GameController
	PlayerPhysicsImproved playerPhysics;								// Holds a reference to the PlayerPhysics script
	Animator animator;													// Holds a reference to the animator component
	Vector3 velocity;													// Holds the velocity of the player

	float accelerationTimeAirborne = .2f;								// The acceleration used while in the air
	float accelerationTimeGrounded = .1f;								// The acceleration used while on the ground
	float timeToWallUnstick;											// Private variable used to set the time the player should unstick from the wall
	float velocityXSmoothing;											// Variable to hold the SmoothDamp of the X velocity
	float maxJumpVelocity;												// The velocity to achieve Max Jump Height
	float minJumpVelocity;												// The velocity to achieve Min Jump Height
	float gravity;														// The gravity

	// TODO Not Used
	float xScale;														// Holds the value of the scale in the X axys

	// TODO Temp: Test of a way to turn
	Transform child;

	void Awake()
	{
		// Gets the PlayerPhysics component
		playerPhysics = GetComponent<PlayerPhysicsImproved>();
		
		// Gets the animator component
		animator = GetComponent<Animator>();
		
		// Check if the Character Controller was found
		if(playerPhysics == null)
		{
			Debug.LogError("PlayerPhysics component not found.");
		}
		
		// Check if the animator component was found
		if(animator == null)
		{
			Debug.LogError("Animator component not found");
		}
		
		// Stores the scale of the player
		xScale = transform.localScale.x;

		// TODO Change this - Not working properly
		child = transform.FindChild("Zuckov");
		xScale = child.localScale.x;
	}

	void Start()
	{
		// Stores a reference to the GameController
		gameController = GameController.instance;

		// Calculate the gravity on the player.
		gravity = - (2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		
		// Sets the global gravity
		gameController.gravity = gravity;
		
		// Calculate the velocity to achieve max Jump Height
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		
		// Calculate the velocity to achieve min Jump Height
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
	}

	void Update()
	{
		// Reset the wallSliding flag
		bool wallSliding = false;
		// Set the direction of the wall if colliding with one
		int wallDirX = (playerPhysics.collisions.left) ? -1 : 1;

		// Set the Attribute Jumping of the animator based on the player position TODO BUG: Not calling the state for jump
		animator.SetBool("Jumping", playerPhysics.collisions.jump);

		// The player input
		Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		// Assigns the acceleration according to whether player is on the ground or not.
		float currentAcceleration = playerPhysics.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne;
		// Calculate the target Velocity
		float targetVelocityX = playerInput.x * moveSpeed;
		// Calculate the X velocity
		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, currentAcceleration);

		// Check if player is colliding with something and is not on ground
		if((playerPhysics.collisions.left || playerPhysics.collisions.right) && !playerPhysics.collisions.below && velocity.y < 0)
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

				// If the player is point to the oposite direction of the wall falloff after the time
				if(playerInput.x != wallDirX && playerInput.x != 0)
				{
					// Reduce the time to Unstick until unstick
					timeToWallUnstick -= Time.deltaTime;
				}
				else
				{
					// Sets the time to unstick the wall
					timeToWallUnstick = wallStickTime;
				}
			}
			else
			{
				// Sets the time to unstick the wall
				timeToWallUnstick = wallStickTime;
			}
		}

		// Starts the Jumping Sequence
		if(Input.GetButtonDown("Jump"))
		{
			if(playerPhysics.collisions.jump && stats.doubleJump)
			{
				Debug.Log("Double Jump");
				velocity.x += doubleJumpVelocity.x * playerPhysics.collisions.faceDir;
				velocity.y = doubleJumpVelocity.y;
			}

			playerPhysics.collisions.jump = true;

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
			if(playerPhysics.collisions.below)
			{
				velocity.y = maxJumpVelocity;
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
		if(playerInput.x != 0 && !wallSliding && !playerPhysics.collisions.draging)
		{
			Flip(Mathf.Sign(playerInput.x));
		}

		// Pass to the animator the current velocity of the player
		animator.SetFloat("Speed", Mathf.Abs(velocity.x));

		// Add the gravity to the Y velocity
		velocity.y += GameController.instance.gravity * Time.deltaTime;	
		// Calls the Move of the player Physics
		playerPhysics.Move(velocity * Time.deltaTime, playerInput);

		// If the player have a collision above or bellow sets the velocity Y to 0
		if(playerPhysics.collisions.below || playerPhysics.collisions.above)
		{
			velocity.y = 0;
		}
	}

	// TODO Figure this out - Should rotate not invert the scale
	void Flip(float dirX)
	{
		Vector3 scale = child.localScale;
		scale.x = dirX * xScale;
		
		child.localScale = scale;

	}

	void OnTriggerEnter(Collider c)
	{
		if(c.tag == "Checkpoint")
		{
			gameController.SetCheckpoint(c.transform.position);
		}
		
		if(c.tag == "Power Up")
		{
			// TODO Pass the Collider/GameObject and Destroy the game object after the pickup
			EnablePowerUp(c.name);
		}
	}
}
