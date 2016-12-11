using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogController {

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
