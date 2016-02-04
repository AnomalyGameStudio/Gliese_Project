using UnityEngine;
using System.Collections;

public class IdleChange : MonoBehaviour {

	// Use this for initialization
	Animator animator;
	
	void Start()
	{
		animator = GetComponent<Animator>();
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			animator.SetFloat("Idle", 1f);
		}
		
		if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			animator.SetFloat("Idle", 2f);
		}
	}
}