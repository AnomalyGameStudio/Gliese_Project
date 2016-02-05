using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour 
{
	private Dictionary <string, UnityEvent> eventDictionary;

	private static EventManager eventManager;

	public static EventManager instance
	{
		get
		{
			// If can't find a EventManager
			if(!eventManager)
			{
				// Search for one
				eventManager = FindObjectOfType(typeof (EventManager)) as EventManager;

				// If still haven't found. There isn't one in the scene
				if(!eventManager)
				{
					// Logs the error
					Debug.LogError("There needs to be one active EventManager Script on a GameObject in your scene.");
				}
				else
				{
					// If just found an Event Manager, It must not be initialized yet, so we initialize it.
					eventManager.Init();
				}
			}

			return eventManager;
		}
	}

	void Init()
	{
		// Initialize the dictionary
		if(eventDictionary == null)
		{
			eventDictionary = new Dictionary<string, UnityEvent>();
		}
	}

	public static void StartListening(string eventName, UnityAction listener)
	{
		UnityEvent thisEvent = null;
		if(instance.eventDictionary.TryGetValue(eventName, out thisEvent))
		{
			// If there is already in the dictionary, adds a listener
			thisEvent.AddListener(listener);
		}
		else
		{
			// If there isn't in the dictionary, creates a new unity Event, adds the listener and adds it to the dictionary
			thisEvent = new UnityEvent();
			thisEvent.AddListener(listener);
			instance.eventDictionary.Add(eventName, thisEvent);
		}
	}

	public static void StopListening(string eventName, UnityAction listener)
	{
		if(eventManager == null) return;

		UnityEvent thisEvent = null;
		// If the listener is on the dictionary stops it
		if(instance.eventDictionary.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.RemoveListener(listener);
		}
	}

	public static void TriggerEvent(string eventName)
	{
		UnityEvent thisEvent = null;
		// If the listener is on the dictionary, invoke
		if(instance.eventDictionary.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.Invoke();
		}
	}
}
