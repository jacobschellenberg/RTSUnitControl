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

	public static void Log(object message, UnityEngine.Object context = null) {
		if (!enableLogging)
			return;

		if (context != null)
			Debug.Log (message, context);
		else
			Debug.Log (message);
	}
}
