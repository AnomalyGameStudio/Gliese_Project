using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider))]
public class BoxGizmos : MonoBehaviour
{
	public Color gizmoColor;

	void OnDrawGizmos()
	{
		BoxCollider collider = GetComponent<BoxCollider>();
		Gizmos.color = gizmoColor;
		Gizmos.DrawCube(transform.position + collider.center, collider.size);
	}
}
