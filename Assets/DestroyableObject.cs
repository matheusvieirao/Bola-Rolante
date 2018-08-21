using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : MonoBehaviour {

	public float forceRequired = 5.0f;

	private void OnCollisionEnter(Collision col){
		Debug.Log (col.impulse.magnitude);
		if (col.impulse.magnitude > forceRequired) {
			Destroy (gameObject);
		}
	}
}
