using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class _Temp_text_debug : MonoBehaviour 
{
	public static _Temp_text_debug instance;

	public Text velX;
	public Text velY;

	void Start()
	{
		instance = this;
	}

	public void SetVelocity(Vector3 velocity)
	{
		velX.text = "Velocity.X: " + velocity.x;
		velY.text = "Velocity.Y: " + velocity.y;
	}
}
