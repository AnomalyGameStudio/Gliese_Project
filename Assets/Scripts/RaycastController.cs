using UnityEngine;
using System.Collections;

//[RequireComponent (typeof(BoxCollider))]
public class RaycastController : MonoBehaviour
{
	//Constants
	public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));				// The Curve used on the descendSlope method
	public const float skinWidth = .015f;																														// The size of the skin in the object

	[HideInInspector] public float horizontalRaySpacing;				// The spacing between the horizontal rays
	[HideInInspector] public float verticalRaySpacing;					// The spacing between the vertical rays
	[HideInInspector] public CapsuleCollider capsuleCollider;			// The capsule Collider of the object
	[HideInInspector] public BoxCollider boxCollider;					// The box collider of the object
	
	public LayerMask collisionMask;										// The layer that will be detected collisions
	public RaycastOrigins raycastOrigins;								// The origins of the Raycasts
	public int horizontalRayCount = 10;									// The number of horizontal rays
	public int verticalRayCount = 10;									// The number of vertical rays

	private bool hasCapsuleCollider;									// Determines if it is a capsule collider or a box collider

	public virtual void Awake()
	{
		// Check if the object have a ChacterController and gets it, Otherwise gets the BoxCollider
		if(GetComponent<CapsuleCollider>() != null)
		{
			// Gets the collider
			capsuleCollider = GetComponent<CapsuleCollider>();
			// Flags as having a capsule collider
			hasCapsuleCollider = true;
		}
		else
		{
			// Gets the collider
			boxCollider = GetComponent<BoxCollider> ();
			// Flags as having a box collider
			hasCapsuleCollider = false;
		}
	}

	public virtual void Start()
	{
		// calculate the spacing between the Rays
		CalculateRaySpacing();
	}

	public void UpdateRaycastOrigins()
	{
		// Gets the bounds of the Character controller if it has one, otherwise gets from the BoxCollider
		Bounds bounds = hasCapsuleCollider? capsuleCollider.bounds : boxCollider.bounds;
		// Adds the Skin to the bounds
		bounds.Expand(skinWidth * -2);

		// Determine all origins used
		raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
	}

	public void CalculateRaySpacing()
	{
		// Gets the bounds of the Character controller if it has one, otherwise gets from the BoxCollider
		Bounds bounds = hasCapsuleCollider? capsuleCollider.bounds : boxCollider.bounds;
		// Adds the Skin to the bounds
		bounds.Expand(skinWidth * -2);

		// Makes sure there is at least 2 rays
		horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

		// Calculate the spacing between the rays based on the size of the collider
		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	public struct RaycastOrigins
	{
		// Stores the origins of the rays
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
}
