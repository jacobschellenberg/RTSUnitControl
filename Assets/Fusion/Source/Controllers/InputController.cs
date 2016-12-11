using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputEvent {
	LeftMouseButtonDown,
	LeftMouseButtonUp,
	LeftMouseButtonDrag,
	RightMouseButtonDown,
	RightMouseButtonUp
}

public class InputController : MonoBehaviour {

	[SerializeField] Camera mainCamera;
	[SerializeField] float timeUntilMouseDrag = 0.002f; // if mouse down helder longer than timer, it's a drag.
	[SerializeField] RectTransform selectionBox;

	int leftMouseButton = 0;
	int rightMouseButton = 1;

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
		#region Left mouse button
		if (Input.GetMouseButtonDown (leftMouseButton)) {
			
			// Get the initial click position of the mouse. No need to convert to GUI space
			// since we are using the lower left as anchor and pivot.
			initialClickPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

			// The anchor is set to the same place.
			selectionBox.anchoredPosition = initialClickPosition;

			isMouseDown = true;
		}

		if (Input.GetMouseButtonUp (leftMouseButton)) {
			if (mouseDownTimer < timeUntilMouseDrag)
				LeftMouseButtonDown (Input.mousePosition);

			LeftMouseButtonUp(Input.mousePosition);

			isMouseDown = false;
			mouseDownTimer = 0;
		}

		if (isMouseDown)
			mouseDownTimer += 0.01f * Time.deltaTime;

		if (mouseDownTimer >= timeUntilMouseDrag)
			LeftMouseButtonDrag (Input.mousePosition);
		#endregion

		#region Right mouse button
		if(Input.GetMouseButtonDown(rightMouseButton))
			RightMouseButtonDown(Input.mousePosition);

		if(Input.GetMouseButtonUp(rightMouseButton))
			RightMouseButtonUp(Input.mousePosition);
		#endregion
	}

	private RaycastHitInfo GetRaycastHitInfo(Vector3 mousePosition)
	{
		RaycastHitInfo raycastHitInfo = null;
		var ray = mainCamera.ScreenPointToRay(mousePosition);
		RaycastHit hitInfo;

		if (Physics.Raycast (ray, out hitInfo, Mathf.Infinity))
			raycastHitInfo = new RaycastHitInfo (hitInfo);

		return raycastHitInfo;
	}

	private void LeftMouseButtonDown(Vector3 mousePosition) {
		var raycastHitInfo = GetRaycastHitInfo (mousePosition);

		EventController.Publish(InputEvent.LeftMouseButtonDown.ToString(), raycastHitInfo);
		LogController.Log ("Left mouse button down: " + mousePosition);
	}

	private void LeftMouseButtonUp(Vector3 mousePosition) {
		var raycastHitInfo = GetRaycastHitInfo (mousePosition);

		// Reset
		initialClickPosition = Vector2.zero;
		selectionBox.anchoredPosition = Vector2.zero;
		selectionBox.sizeDelta = Vector2.zero;

		EventController.Publish(InputEvent.LeftMouseButtonUp.ToString(), raycastHitInfo);
		LogController.Log ("Left mouse button up: " + mousePosition);
	}

	private void LeftMouseButtonDrag(Vector3 mousePosition) {
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

//		EventController.Publish("LeftMouseButtonDrag", raycastHitInfo);
		LogController.Log ("Left mouse button drag: " + mousePosition);
	}

	private void RightMouseButtonDown(Vector3 mousePosition) {
		var raycastHitInfo = GetRaycastHitInfo (mousePosition);

		EventController.Publish(InputEvent.RightMouseButtonDown.ToString(), raycastHitInfo);
		LogController.Log ("Right mouse button down: " + mousePosition);
	}

	private void RightMouseButtonUp(Vector3 mousePosition) {
		var raycastHitInfo = GetRaycastHitInfo (mousePosition);

		EventController.Publish(InputEvent.RightMouseButtonUp.ToString(), raycastHitInfo);
		LogController.Log ("Right mouse button up: " + mousePosition);
	}
}
