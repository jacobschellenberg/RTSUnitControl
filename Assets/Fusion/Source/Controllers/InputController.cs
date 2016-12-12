﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

	int leftMouseButton = 0;
	int rightMouseButton = 1;

	float mouseDownTimer;
	bool isMouseDown;
	bool mouseUpFromDrag;

	// Draggable inspector reference to the Image GameObject's RectTransform.
	public RectTransform selectionBox;

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


		LogController.Log ("Raycast hit point: " + hitInfo.point);
		return raycastHitInfo;
	}

	private List<RaycastHitInfo> GetRaycastHitInfos(Vector2 mousePosition)
	{
		var raycastHitInfos = new List<RaycastHitInfo> ();

//		var hitInfos = Physics.BoxCastAll(

//
//		foreach (var hitInfo in hitInfos) {
//			var raycastHitInfo = new RaycastHitInfo (hitInfo);
//			raycastHitInfos.Add (raycastHitInfo);
//		}

		LogController.Log ("BoxCast hits: " + raycastHitInfos.Count);
		return raycastHitInfos;
	}

	private void LeftMouseButtonDown(Vector3 mousePosition) {
		var raycastHitInfo = GetRaycastHitInfo (mousePosition);

		EventController.Publish(InputEvent.LeftMouseButtonDown.ToString(), raycastHitInfo);
		LogController.Log ("Left mouse button down: " + mousePosition);
	}

	private void LeftMouseButtonUp(Vector2 mousePosition) {
		var raycastHitInfos = new List<RaycastHitInfo>();

		if (mouseUpFromDrag) {
//			raycastHitInfos = GetRaycastHitInfos (mousePosition);

			var startPoint = GetRaycastHitInfo (initialClickPosition);
			var lastPoint = GetRaycastHitInfo (mousePosition);
			var midPoint = new Vector3 ((startPoint.Point.x + lastPoint.Point.x) / 2, 0, (startPoint.Point.z + lastPoint.Point.z) / 2);

			var x1 = startPoint.Point.x > lastPoint.Point.x ? startPoint.Point.x : lastPoint.Point.x;
			var x2 = startPoint.Point.x > lastPoint.Point.x ? lastPoint.Point.x : startPoint.Point.x;

			var y1 = startPoint.Point.z > lastPoint.Point.z ? startPoint.Point.z : lastPoint.Point.z;
			var y2 = startPoint.Point.z > lastPoint.Point.z ? lastPoint.Point.z : startPoint.Point.z;

			LogController.Log ("Mouse position: " + mousePosition + " Initial position: " + initialClickPosition);
			LogController.Log ("x1: " + x1 + " x2: " + x2);
			LogController.Log ("y1: " + y1 + " y2: " + y2);

			var size = new Vector3(startPoint.Point.x - lastPoint.Point.x, 1, startPoint.Point.z - lastPoint.Point.z);

			LogController.Log ("Start point: " + startPoint.Point);
			LogController.Log ("Last point: " + lastPoint.Point);
			LogController.Log ("Mid point: " + midPoint);
			LogController.Log ("Box size: " + size);

			var point = GameObject.CreatePrimitive (PrimitiveType.Cube);
			point.transform.localScale = size;
			point.transform.position = midPoint;
		}
		else
			raycastHitInfos.Add(GetRaycastHitInfo (mousePosition));

		mouseUpFromDrag = false; // reset for next mouse up event

		// Reset
		initialClickPosition = Vector2.zero;
		selectionBox.anchoredPosition = Vector2.zero;
		selectionBox.sizeDelta = Vector2.zero;

		EventController.Publish(InputEvent.LeftMouseButtonUp.ToString(), raycastHitInfos);
		LogController.Log ("Left mouse button up: " + mousePosition);
	}

	private void LeftMouseButtonDrag(Vector3 mousePosition) {
		mouseUpFromDrag = true;

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
