using UnityEngine;
using System.Collections;

public class MovementController : RaycastController 
{
	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
	public float moveSpeed = 1;

	float accelerationAirborne = .2f;
	float accelerationGrounded = .1f;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	float velocityXSmoothing;

	Vector3 velocity;

	CollisionInfo collisionInfo;
	GameController gameController;

	public struct CollisionInfo
	{
		public bool left, right;
		public bool above, bellow;

		public void Reset()
		{
			left = right = false;
			above = bellow = false;
		}
	}

	void Start()
	{
		gameController = GameController.instance;
		collisionInfo = new CollisionInfo();

		gravity = - (2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
		
		gameController.gravity = gravity;
	}

	void Update()
	{
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		float currentAcceleration = collisionInfo.bellow ? accelerationGrounded : accelerationAirborne;
		float targetVelocityX = input.x * moveSpeed;

		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, currentAcceleration);

		if(Input.GetButtonDown("Jump"))
		{
			if(collisionInfo.bellow)
			{

				velocity.y = maxJumpVelocity;
			}
		}
		else if(Input.GetButtonUp("Jump"))
		{
			if(velocity.y > minJumpVelocity)
			{
				velocity.y = minJumpVelocity;
			}
		}

		velocity.y += gravity * Time.deltaTime;
		Move(velocity * Time.deltaTime);
	}

	public void Move(Vector3 velocity)
	{
		UpdateRaycastOrigins();
		collisionInfo.Reset();

		HorizontalCollisions(ref velocity);
		Debug.Log(collisionInfo.bellow);
		if(velocity.y != 0)
		{
			VerticalCollisions(ref velocity);
		}

		transform.Translate(velocity, Space.World);
	}

	void HorizontalCollisions(ref Vector3 velocity)
	{
		RaycastHit hit;
		Ray ray;

		float directionX = Mathf.Sign(velocity.x);
		float rayLenght = Mathf.Abs(velocity.x) + skinWidth;

		//Caso esteja parado cria um Ray do tamanho da Skin para continuar detectando colisoes em pulos simples
		if(Mathf.Abs(velocity.x) < skinWidth)
		{
			rayLenght = 2 * skinWidth;
		}

		for(int i = 0; i < horizontalRayCount; i++)
		{
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight; //TODO pegar os bounds
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);

			ray = new Ray(rayOrigin, Vector3.right * directionX);
			Debug.DrawRay(ray.origin, ray.direction, Color.red);

			if(Physics.Raycast(ray, out hit, rayLenght, collisionMask))
			{
				velocity.x = (hit.distance - skinWidth) * directionX;
				//rayLenght = hit.distance; //TODO determinar pra que serve esse ponto

				collisionInfo.left = directionX == 1;
				collisionInfo.right = directionX == -1;
			}
		}
	}

	void VerticalCollisions(ref Vector3 velocity)
	{
		RaycastHit hit;
		Ray ray;
		
		float directionY = Mathf.Sign(velocity.y);
		float rayLenght = Mathf.Abs(velocity.y) + skinWidth;
		
		//Caso esteja parado cria um Ray do tamanho da Skin para continuar detectando colisoes em pulos simples
		if(Mathf.Abs(velocity.y) < skinWidth)
		{
			rayLenght = 2 * skinWidth;
		}

		for(int i = 0; i < verticalRayCount; i++)
		{
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft; //TODO pegar os bounds
			rayOrigin += Vector2.right * (verticalRaySpacing * i);
			
			ray = new Ray(rayOrigin, Vector3.up * directionY);
			
			Debug.DrawRay(ray.origin, ray.direction, Color.red);

			if(Physics.Raycast(ray, out hit, rayLenght, collisionMask))
			{
				velocity.y = (hit.distance - skinWidth) * directionY;
				//rayLenght = hit.distance; //TODO determinar pra que serve esse ponto

				collisionInfo.above = directionY == 1;
				collisionInfo.bellow = directionY == -1;
			}
			
		}
	}
}
