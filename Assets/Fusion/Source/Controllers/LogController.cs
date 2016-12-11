using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LogController : MonoBehaviour {

	private static LogController instance;
	public static LogController Instance {
		get { 
			if (instance == null)
				instance = GameObject.FindGameObjectWithTag ("LogController").GetComponent<LogController> () as LogController;

			return instance;
		}
	}

	public static bool enableLogging = true;

	void Start() {
		EventController.Subscribe<RaycastHitInfo> (new Subscription<RaycastHitInfo>("OnClick", OnClickDown));
	}

	public static void Log(object message, UnityEngine.Object context = null) {
		if (!enableLogging)
			return;

		if (context != null)
			Debug.Log (message, context);
		else
			Debug.Log (message);
	}

	void OnClickDown(object sender, EventMessage<RaycastHitInfo> e){
		Log ("Clicked: " + e.Payload.Name);
	}
}
