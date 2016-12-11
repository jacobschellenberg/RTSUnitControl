using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorViewController : MonoBehaviour {

	protected virtual void Start() {
		EventController.Subscribe (new Subscription<RaycastHitInfo> ("MouseDown", OnClickDown));
	}

	protected virtual void OnClickDown(object sender, EventMessage<RaycastHitInfo> e) {} 
}
