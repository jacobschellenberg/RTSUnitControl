using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class ActorViewController : MonoBehaviour {

	public PlayerController Player { get; private set; }
	public bool IsSelected { get; private set; }

	[SerializeField] private float speed;
	[SerializeField] private Transform selectedCircle;
	[SerializeField] private float defaultStopAtDistance = 0.1f;
	[SerializeField] private float visionDistance = 3;
	[SerializeField] private float followDistance = 1.5f;
	[SerializeField] private float tooCloseDistance = 1;

	private Transform target;
	private Vector3 targetPosition;
	private bool atPosition = false;
	private bool targetPositionSet = false;
	private Renderer meshRenderer;
	private float stopAtDistance = 0.1f;

	private Vector3 rayOrigin;
	private Ray ray;
	private RaycastHit raycastHitInfo;
	private bool tooClose;
	ActorViewController closestActor = null;
	private float closest = 0;

	protected virtual void Awake() {
		meshRenderer = this.transform.GetComponent<Renderer> ();
	}

	protected virtual void Update() { 
		if(closestActor != null && IsSelected)
			LogController.Log ("Closes Actor: " + closestActor.name);

		if (!IsSelected && !targetPositionSet && target == null)
			return;

		rayOrigin = this.transform.position;
		ray = new Ray (rayOrigin, this.transform.forward);
		Debug.DrawRay (rayOrigin, (this.transform.forward * visionDistance));

		var overlapActors = Physics.OverlapSphere (this.transform.position, visionDistance)
			.Where(_ => _.GetComponent<ActorViewController>() != null)
			.Select (_ => _.GetComponent<ActorViewController> ())
			.Where(_ => _ != this);

		if (overlapActors.Any ()) {
			var nearActors = overlapActors.Where (_ => Vector3.Distance (this.transform.position, _.transform.position) < tooCloseDistance).ToList ();

			if (nearActors.Any ()) {
				foreach (var actor in nearActors) {
					var distance = Vector3.Distance (this.transform.position, actor.transform.position);

					if (closestActor == null) {
						closestActor = actor;
						closest = distance;
					}

					if (distance < closest) {
						closestActor = actor;
						closest = distance;
					}
				}

				tooClose = true;
			} else {
				stopAtDistance = defaultStopAtDistance;
				closestActor = null;
				closest = 0;
				tooClose = false;
			}
		} else {
			stopAtDistance = defaultStopAtDistance;
			closestActor = null;
			closest = 0;
			tooClose = false;
		}

		if (target != null && Vector3.Distance (this.transform.position, target.position) > stopAtDistance) {
			targetPosition = target.transform.position;
			atPosition = false;
			targetPositionSet = true;
		}

		if ((Vector3.Distance (this.transform.position, targetPosition) < stopAtDistance) || (target != null && Vector3.Distance (this.transform.position, targetPosition) < followDistance)) {
			atPosition = true;
			targetPositionSet = false;
		}

		if (targetPositionSet && !atPosition) {
			this.transform.LookAt (targetPosition);

			// TODO: cheated using rigidbody... fix that.
//			if (tooClose) {
//				Vector3 dir = transform.position - closestActor.transform.position;
//				this.transform.Translate(dir * (speed * 1.5f) * Time.deltaTime);
//			}
//			else
				this.transform.position = Vector3.MoveTowards (this.transform.position, targetPosition, speed * Time.deltaTime);
		}
	}

	public void MoveTo(Vector3 position)
	{
		targetPosition = position;
		atPosition = false;
		targetPositionSet = true;
		target = null;
		LogController.Log(this.name + " moving to: " + targetPosition);
	}

	public void GoToTarget(Transform target) {
		this.target = target;
		atPosition = false;
		targetPositionSet = true;
		LogController.Log(this.name + " moving to: " + target.name);
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
