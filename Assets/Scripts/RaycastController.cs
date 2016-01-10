using UnityEngine;
using System.Collections;

//[RequireComponent (typeof(BoxCollider))]
public class RaycastController : MonoBehaviour
{
	//Constants
	public const float skinWidth = .015f;

	[HideInInspector]
	public float horizontalRaySpacing;
	[HideInInspector]
	public float verticalRaySpacing;
	[HideInInspector]
	public CharacterController controller;
	//public BoxCollider collider;
	
	public LayerMask collisionMask;
	public RaycastOrigins raycastOrigins;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

	public virtual void Awake()
	{
		controller = GetComponent<CharacterController>();
		//collider = GetComponent<BoxCollider> ();
	}

	public virtual void Start()
	{
		CalculateRaySpacing();
	}

	public void UpdateRaycastOrigins()
	{
		Bounds bounds = controller.bounds;
		bounds.Expand(skinWidth * -2);
		
		raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
	}

	public void CalculateRaySpacing()
	{
		Bounds bounds = controller.bounds;
		bounds.Expand(skinWidth * -2);
		
		horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);
		
		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	public struct RaycastOrigins
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
}
