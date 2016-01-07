using UnityEngine;
using System.Collections;

[RequireComponent (typeof(PlayerPhysicsImproved))]
public class PlayerControllerImproved : Entity 
{
	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;

	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
	public float wallSlideSpeedMax = 3;
	public float wallStickTime = .25f;

	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	float moveSpeed = 6;
	float timeToWallUnstick;
	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	float velocityXSmoothing;

	Vector3 velocity;
	PlayerPhysicsImproved playerPhysics;
	GameController gameController;

	void Start()
	{
		gameController = GameController.instance;
		playerPhysics = GetComponent<PlayerPhysicsImproved>();

		gravity = - (2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

		gameController.gravity = gravity;
	}

	void Update()
	{
		bool wallSliding = false;
		int wallDirX = (playerPhysics.collisions.left) ? -1 : 1;

		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		float currentAcceleration = playerPhysics.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne;
		float targetVelocityX = input.x * moveSpeed;

		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, currentAcceleration);

		if((playerPhysics.collisions.left || playerPhysics.collisions.right) && !playerPhysics.collisions.below && velocity.y < 0)
		{
			wallSliding = true;
			
			if(velocity.y < -wallSlideSpeedMax)
			{
				velocity.y = -wallSlideSpeedMax;
			}
			
			if(timeToWallUnstick > 0)
			{
				velocityXSmoothing = 0;
				velocity.x = 0;
				
				if(input.x != wallDirX && input.x != 0)
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
		
		if(Input.GetButtonDown("Jump"))
		{
			if(wallSliding)
			{
				if(wallDirX == input.x)
				{
					velocity.x = -wallDirX * wallJumpClimb.x;
					velocity.y = wallJumpClimb.y;
				}
				else if (input.x == 0)
				{
					velocity.x = -wallDirX * wallJumpOff.x;
					velocity.y = wallJumpOff.y;
				}
				else
				{
					velocity.x = -wallDirX * wallLeap.x;
					velocity.y = wallLeap.y;
				}
			}
			if(playerPhysics.collisions.below)
			{
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

		velocity.y += gravity * Time.deltaTime;
		playerPhysics.Move(velocity * Time.deltaTime, input);
		
		if(playerPhysics.collisions.below || playerPhysics.collisions.above)
		{
			velocity.y = 0;
		}
	}

	void OnTriggerEnter(Collider c)
	{
		if(c.tag == "Checkpoint")
		{
			gameController.SetCheckpoint(c.transform.position);
		}
		
		if(c.tag == "Finish")
		{
			gameController.EndLevel();
		}
	}
}
