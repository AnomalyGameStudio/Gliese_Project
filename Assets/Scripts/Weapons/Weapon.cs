using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour, IWeapon
{
	public LayerMask toHit;
	public float fireRate;
	float timeToFire = 0;

	private Transform weaponFirePoint;

	public Transform firePoint
	{
		get
		{
			return weaponFirePoint;
		}

		set
		{
			weaponFirePoint = value;
		}
	}

	void Awake()
	{
		firePoint = transform.FindChild("FirePoint");

		if(firePoint == null)
		{
			Debug.LogError("Firepoint object not found.");
		}
	}

	void Update()
	{
		if(fireRate == 0)
		{
			if(Input.GetButtonDown("Fire1"))
			{
				Debug.Log("shoot");
				Shoot();
			}
		}
		else
		{
			if(Input.GetButton("Fire1") && Time.time > timeToFire)
			{
				timeToFire = Time.time + 1/fireRate;
				Shoot();
			}
		}
	}

	public void Shoot()
	{
		Vector3 mousePos = Input.mousePosition;
		mousePos.z = Camera.main.nearClipPlane;

		Vector3 screenMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 mousePosition = new Vector2(screenMousePosition.x, screenMousePosition.y);
		Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);

		Debug.Log(firePointPosition);

		RaycastHit hit;
		Ray ray;

		ray = new Ray(firePointPosition, (mousePosition - firePointPosition));

		Debug.Log("Direction: " + ray.direction + " Mouse Position: " + mousePosition + " firePointPosition: " +  firePointPosition);

		Debug.DrawLine(ray.origin, ray.direction , Color.cyan);

		if(Physics.Raycast(ray, out hit, 100, toHit))
		{
			Debug.DrawLine(firePointPosition, hit.point, Color.red);
		}
	}
}
