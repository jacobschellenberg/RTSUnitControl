using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputController : MonoBehaviour {

	private static InputController instance;
	public static InputController Instance {
		get { 
			if (instance == null)
				instance = GameObject.FindGameObjectWithTag ("InputController").GetComponent<InputController> () as InputController;

			return instance;
		}
	}

	[SerializeField] Camera mainCamera;

	void Start() {
		if (mainCamera == null && Camera.main != null)
			mainCamera = Camera.main;
	}

	void Update() {
		if (Input.GetMouseButtonDown (0))
			ClickDown (Input.mousePosition);
	}

	public RaycastHitInfo ClickDown(Vector3 mousePosition) {
		RaycastHitInfo raycastHitInfo = null;
		var ray = mainCamera.ScreenPointToRay(mousePosition);
		RaycastHit hitInfo;

		if (Physics.Raycast (ray, out hitInfo, Mathf.Infinity)) {
			raycastHitInfo = new RaycastHitInfo (hitInfo);
		}

		EventController.Publish("OnClick", raycastHitInfo);
		return raycastHitInfo;
	}
}
