using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorViewController : MonoBehaviour {

	protected virtual void Start() {
		EventController.Subscribe (new Subscription<RaycastHitInfo> (InputEvent.LeftMouseButtonDown.ToString(), OnLeftMouseButtonDown));
		EventController.Subscribe (new Subscription<RaycastHitInfo> (InputEvent.RightMouseButtonDown.ToString(), OnRightMouseButtonDown));
	}

	protected virtual void OnLeftMouseButtonDown(object sender, EventMessage<RaycastHitInfo> e) {} 
	protected virtual void OnRightMouseButtonDown(object sender, EventMessage<RaycastHitInfo> e) {} 
}
