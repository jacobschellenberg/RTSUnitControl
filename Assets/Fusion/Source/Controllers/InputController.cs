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
	[SerializeField] float timeUntilMouseDrag = 0.002f; // if mouse down helder longer than timer, it's a drag.
	[SerializeField] RectTransform selectionBox;

	float mouseDownTimer;
	bool isMouseDown;

	// This variable will store the location of wherever we first click before dragging.
	private Vector2 initialClickPosition = Vector2.zero;

	void Start() {
		if (mainCamera == null && Camera.main != null)
			mainCamera = Camera.main;

		if (selectionBox == null)
			LogController.Log ("Assign a dragbox to the InputController.");
	}

	void Update() {
		if (Input.GetMouseButtonDown (0)) {
			// Get the initial click position of the mouse. No need to convert to GUI space
			// since we are using the lower left as anchor and pivot.
			initialClickPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

			// The anchor is set to the same place.
			selectionBox.anchoredPosition = initialClickPosition;

			isMouseDown = true;
		}

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
			MouseDrag (Input.mousePosition);
	}

	public RaycastHitInfo MouseDown(Vector3 mousePosition) {
		RaycastHitInfo raycastHitInfo = null;
		var ray = mainCamera.ScreenPointToRay(mousePosition);
		RaycastHit hitInfo;

		if (Physics.Raycast (ray, out hitInfo, Mathf.Infinity)) {
			raycastHitInfo = new RaycastHitInfo (hitInfo);
		}

		EventController.Publish("MouseDown", raycastHitInfo);
		LogController.Log ("Mouse Down: " + mousePosition);
		return raycastHitInfo;
	}

	public void MouseUp(Vector3 mousePosition) {
		// Reset
		initialClickPosition = Vector2.zero;
		selectionBox.anchoredPosition = Vector2.zero;
		selectionBox.sizeDelta = Vector2.zero;

		LogController.Log ("Mouse Up: " + mousePosition);
	}

	public void MouseDrag(Vector3 position) {
		// Store the current mouse position in screen space.
		Vector2 currentMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

		// How far have we moved the mouse?
		Vector2 difference = currentMousePosition - initialClickPosition;

		// Copy the initial click position to a new variable. Using the original variable will cause
		// the anchor to move around to wherever the current mouse position is,
		// which isn't desirable.
		Vector2 startPoint = initialClickPosition;

		// The following code accounts for dragging in various directions.
		if (difference.x < 0)
		{
			startPoint.x = currentMousePosition.x;
			difference.x = -difference.x;
		}
		if (difference.y < 0)
		{
			startPoint.y = currentMousePosition.y;
			difference.y = -difference.y;
		}

		// Set the anchor, width and height every frame.
		selectionBox.anchoredPosition = startPoint;
		selectionBox.sizeDelta = difference;

		LogController.Log ("Mouse Drag: " + position);
	}
}
