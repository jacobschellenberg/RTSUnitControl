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
	[SerializeField] float timeUntilMouseDrag = 0.004f; // if mouse down helder longer than timer, it's a drag.

	float mouseDownTimer;
	bool isMouseDown;

	void Start() {
		if (mainCamera == null && Camera.main != null)
			mainCamera = Camera.main;
	}

	void Update() {
		if (Input.GetMouseButtonDown (0))
			isMouseDown = true;

		if (Input.GetMouseButtonUp (0)) {
			if (mouseDownTimer < timeUntilMouseDrag)
				MouseDown (Input.mousePosition);

			MouseUp(Input.mousePosition);

			isMouseDown = false;
			mouseDownTimer = 0;
		}

		if (isMouseDown)
			mouseDownTimer += 0.01f * Time.deltaTime;

		if (mouseDownTimer >= timeUntilMouseDrag)
			MouseDrag ();
	}

	public RaycastHitInfo MouseDown(Vector3 mousePosition) {
		RaycastHitInfo raycastHitInfo = null;
		var ray = mainCamera.ScreenPointToRay(mousePosition);
		RaycastHit hitInfo;

		if (Physics.Raycast (ray, out hitInfo, Mathf.Infinity)) {
			raycastHitInfo = new RaycastHitInfo (hitInfo);
		}

		EventController.Publish("MouseDown", raycastHitInfo);
		LogController.Log ("Mouse Down");
		return raycastHitInfo;
	}

	public void MouseUp(Vector3 mousePosition) {
		LogController.Log ("Mouse Up");
	}

	public void MouseDrag() {
		LogController.Log ("Mouse Drag");
	}
}
