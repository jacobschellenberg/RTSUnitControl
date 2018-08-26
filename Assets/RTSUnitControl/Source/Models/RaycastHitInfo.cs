using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastHitInfo {

	public Collider Collider { get; private set; }
	public float Distance { get; private set;}
	public Vector3 Normal { get; private set; }
	public Vector3 Point { get; private set; }
	public Rigidbody Rigidbody { get; private set; }
	public Transform Transform { get; private set; }
	public GameObject GameObject { get; private set; }
	public string Name { get; private set; }
	public string Tag { get; private set; }

	public RaycastHitInfo(RaycastHit raycastHit) 
	{
		Collider = raycastHit.collider;
		Distance = raycastHit.distance;
		Normal = raycastHit.normal;
		Point = raycastHit.point;
		Rigidbody = raycastHit.rigidbody;
		Transform = raycastHit.transform;
		GameObject = raycastHit.transform.gameObject;
		Name = raycastHit.transform.name;
		Tag = raycastHit.transform.tag;
	}
}
