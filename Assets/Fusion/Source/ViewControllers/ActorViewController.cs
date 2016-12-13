using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorViewController : MonoBehaviour {

	public PlayerController Player { get; private set; }
	public bool IsSelected { get; private set; }

	[SerializeField] private float speed;
	[SerializeField] private Transform selectedCircle;

	private Vector3 targetPosition;
	private bool atPosition = false;
	private bool targetPositionSet = false;
	private Renderer meshRenderer;

	protected virtual void Awake() {
		meshRenderer = this.transform.GetComponent<Renderer> ();
	}

	protected virtual void Update() { 
		if (!IsSelected && !targetPositionSet)
			return;

		if (Vector3.Distance (this.transform.position, targetPosition) < 0.1f) { 
			atPosition = true;
			targetPositionSet = false;
		}

		if (targetPositionSet && !atPosition)
			this.transform.position = Vector3.MoveTowards (this.transform.position, targetPosition, speed * Time.deltaTime);
	}

	public void MoveTo(Vector3 position)
	{
		targetPosition = position;
		atPosition = false;
		targetPositionSet = true;
		LogController.Log(this.name + " moving to: " + targetPosition);
	}

	public void Select() {
		IsSelected = true;
//		meshRenderer.material.color = Color.green;
		selectedCircle.gameObject.SetActive(true);
		LogController.Log (this.name + " selected.");
	}

	public void Deselect() {
		IsSelected = false;
//		meshRenderer.material.color = Player.TeamSettings.Color;
		selectedCircle.gameObject.SetActive(false);
		LogController.Log (this.name + " deselected.");
	}

	public void SetPlayer(PlayerController player) {
		Player = player;

		meshRenderer.material.color = Player.TeamSettings.Color;
	}
}
