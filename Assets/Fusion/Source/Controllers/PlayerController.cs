using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour {


	public int TeamId { get; set; }
	public List<ActorViewController> Units { get; private set; }
	public TeamSettings TeamSettings { get; private set; }

	[SerializeField] private int teamId;
	[SerializeField] private List<ActorViewController> units;

	private void Awake() {
		TeamId = teamId;
		Units = units.Any () ? units : new List<ActorViewController> ();
	}

	private void OnLeftMouseButtonDown(object sender, EventMessage<RaycastHitInfo> e) {
		var actor = GetActorFromPayload (e.Payload);

		DeselectAllActors ();

		// if nothing is selected, deslect everything.
		if (actor != null && actor.Player.TeamSettings.ID == TeamSettings.ID)
			SelectActor (actor);
	}

	private void OnLeftMouseButtonUp(object sender, EventMessage<List<RaycastHitInfo>> e) {
		var actors = GetActorsFromPayload (e.Payload).Where(_ => _.Player.TeamSettings.ID == TeamSettings.ID).ToList();

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

	public void SetTeam(TeamSettings teamSettings) {
		TeamSettings = teamSettings;

		Units.ForEach (_ => {
			_.SetPlayer(this);
		});

		if (TeamSettings.ID == GameController.Instance.ClientPlayer.TeamSettings.ID) {
			EventController.Subscribe (new Subscription<RaycastHitInfo> (InputEvent.LeftMouseButtonDown.ToString (), OnLeftMouseButtonDown));
			EventController.Subscribe (new Subscription<List<RaycastHitInfo>> (InputEvent.LeftMouseButtonUp.ToString (), OnLeftMouseButtonUp));
			EventController.Subscribe (new Subscription<RaycastHitInfo> (InputEvent.RightMouseButtonDown.ToString (), OnRightMouseButtonDown));
			EventController.Subscribe (new Subscription<List<RaycastHitInfo>> (InputEvent.ShiftLeftMouseButtonUp.ToString (), OnShiftLeftMouseButtonUp));
		}
	}
}
