using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyItem : MonoBehaviour {
	public void DestroyObject () {
		GameObject.DestroyImmediate(gameObject);
	}
	public void DestroyObject(float timer) {
		Invoke("DestroyObject", timer);
	}
}
