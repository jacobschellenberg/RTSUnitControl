using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour {

	public List<ActorViewController> Units { get; private set; }

	[SerializeField] private List<ActorViewController> units;

	private void Awake() {
		Units = units.Any () ? units : new List<ActorViewController> ();

		EventController.Subscribe (new Subscription<RaycastHitInfo> (InputEvent.LeftMouseButtonDown.ToString(), OnLeftMouseButtonDown));
		EventController.Subscribe (new Subscription<List<RaycastHitInfo>> (InputEvent.LeftMouseButtonUp.ToString(), OnLeftMouseButtonUp));
		EventController.Subscribe (new Subscription<RaycastHitInfo> (InputEvent.RightMouseButtonDown.ToString(), OnRightMouseButtonDown));
		EventController.Subscribe (new Subscription<List<RaycastHitInfo>> (InputEvent.ShiftLeftMouseButtonUp.ToString(), OnShiftLeftMouseButtonUp));
	}

	private void OnLeftMouseButtonDown(object sender, EventMessage<RaycastHitInfo> e) {
		var actor = GetActorFromPayload (e.Payload);

		DeselectAllActors ();

		// if nothing is selected, deslect everything.
		if (actor != null)
			SelectActor (actor);
	}

	private void OnLeftMouseButtonUp(object sender, EventMessage<List<RaycastHitInfo>> e) {
		var actors = GetActorsFromPayload (e.Payload);

		DeselectAllActors();
		SelectActors (actors);
	}

	private void OnRightMouseButtonDown(object sender, EventMessage<RaycastHitInfo> e) {
		if (e.Payload.Tag != "Ground")
			return;
		
		var selectedActors = GetSelectedActors ();

		selectedActors.ForEach (_ => {
			_.MoveTo(new Vector3(e.Payload.Point.x, this.transform.position.y, e.Payload.Point.z));
		});
	}

	private void OnShiftLeftMouseButtonUp(object sender, EventMessage<List<RaycastHitInfo>> e) {
		var actors = GetActorsFromPayload (e.Payload);
		var selectedActors = GetSelectedActors ();

		if (selectedActors.ContainsAll (actors)) {
			DeselectActors (actors);
		} else {
			SelectActors (actors);
		}
	}

	private List<ActorViewController> GetSelectedActors() {
		return Units.Where (_ => _.IsSelected).ToList();
	}

	private void SelectActor(ActorViewController actor) {
		actor.Select ();
	}

	private void SelectActors(List<ActorViewController> actors) {
		actors.ForEach (_ => {
			_.Select();
		});
	}

	private void DeselectActor(ActorViewController actor) {
		actor.Deselect ();
	}

	private void DeselectActors(List<ActorViewController> actors) {
		actors.ForEach (_ => {
			_.Deselect ();
		});	
	}

	private void DeselectAllActors() {
		DeselectActors (Units);
	}

	private ActorViewController GetActorFromPayload(RaycastHitInfo payload) {
		return payload.Transform.GetComponent<ActorViewController> ();
	}

	private List<ActorViewController> GetActorsFromPayload(List<RaycastHitInfo> payload) {
		return payload.Where (_ => _.Transform.GetComponent<ActorViewController> () != null)
			.Select (_ => _.Transform.GetComponent<ActorViewController> ()).ToList();
	}
}
