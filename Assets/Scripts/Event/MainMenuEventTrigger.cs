using UnityEngine;
using System.Collections;

public class MainMenuEventTrigger : MonoBehaviour 
{
	public void StartOnClick()
	{
		EventManager.TriggerEvent("NewGame");
	}

	public void ExitButton()
	{
		EventManager.TriggerEvent("Exit");
	}
}
