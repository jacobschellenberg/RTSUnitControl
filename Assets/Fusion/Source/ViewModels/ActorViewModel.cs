using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorViewModel : MonoBehaviour {

	[SerializeField] float speed;

	Vector3 targetPosition;
	bool atPosition = false;
	bool selected = false;
	bool targetPositionSet = false;
	Renderer meshRenderer;

	void Start() {
		meshRenderer = this.transform.GetComponent<Renderer> ();
		EventController.Subscribe (new Subscription<RaycastHitInfo> ("MouseDown", OnClickDown));
	}

	void Update() {
		if (!selected && !targetPositionSet)
			return;
		
		if (Vector3.Distance (this.transform.position, targetPosition) < 0.1f) { 
			atPosition = true;
			targetPositionSet = false;
		}

		if (targetPositionSet && !atPosition)
			this.transform.position = Vector3.MoveTowards (this.transform.position, targetPosition, speed * Time.deltaTime);
	}

	public void OnClickDown(object sender, EventMessage<RaycastHitInfo> e) {
		var actor = e.Payload.Transform.GetComponent<ActorViewModel> ();

		if (actor != null && actor.transform == this.transform)
			selected = true;
		else if (actor != null && actor.transform != this.transform)
			selected = false;
		else if(selected && e.Payload.Tag == "Ground")
			MoveTo(new Vector3(e.Payload.Point.x, this.transform.position.y, e.Payload.Point.z));

		if (selected) {
			LogController.Log (this.name + " selected.");
			meshRenderer.material.color = Color.green;
		}
		else
			meshRenderer.material.color = Color.white;
	}

	public void MoveTo(Vector3 position)
	{
		targetPosition = position;
		atPosition = false;
		targetPositionSet = true;
		LogController.Log(this.name + " moving to: " + targetPosition);
	}
}
