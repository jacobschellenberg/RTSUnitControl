using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

	[SerializeField] private float rotationSpeed = 100f;
	[SerializeField] private Vector3 rotationAxis = Vector3.up;

	private void Update() {
		this.transform.Rotate (rotationAxis, rotationSpeed * Time.deltaTime);
	}

}
