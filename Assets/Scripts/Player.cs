using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour 
{
	public float jumpHeight = 4;
	public float timeToJumpApex = .4f;

	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	float moveSpeed = 6;

	float gravity;
	float jumpVelocity;
	float velocityXSmoothing;

	Vector3 velocity;
	Controller2D controller;

	void Start()
	{
		controller = GetComponent<Controller2D> ();

		gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		Debug.Log("Gravity: " + gravity + " JumpVelocity: " + jumpVelocity);
	}	

	void Update()
	{
		if(controller.collisions.below || controller.collisions.above)
		{
			velocity.y = 0;
		}

		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		if(Input.GetButtonDown("Jump") && controller.collisions.below)
		{
			velocity.y = jumpVelocity;
		}

		float currentAcceleration = controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne;
		float targetVelocityX = input.x * moveSpeed;

		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, currentAcceleration);
		velocity.y += gravity * Time.deltaTime;
		controller.Move(velocity * Time.deltaTime);

	}
}
