using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GUITextureLoader))]
public class FaceManager : MonoBehaviour {

	private const float timeToStartLooking = 10;
	private const float lookDelay = 2;

	[SerializeField] private Face[] faces = null;

	private GUITextureLoader textureLoader;
	private Coroutine lookCoroutine;
	private bool isFaceOverriding = false;
	int idx = 0;

	// Use this for initialization
	void Start () {
		textureLoader = GetComponent<GUITextureLoader>();
		System.Array.Sort(faces, delegate(Face x, Face y) { return y.healthAmount.CompareTo(x.healthAmount); });
		lookCoroutine = StartCoroutine(Look());
	}
	
	public void UpdateFace (int health)
	{
		StopCoroutine(lookCoroutine);
		for (int i = 0; i < faces.Length; i++) {
			if (faces [i].healthAmount <= health) {
				idx = i;
				break;
			}
		}
		if (!isFaceOverriding) {
			SetFaceSprite(faces[idx].centre);
			lookCoroutine = StartCoroutine(Look());
		}
	}

	public void SetFaceOverride(string id, float time) {
		StopCoroutine(lookCoroutine);
		SetFaceSprite(id);
		StartCoroutine(CancelOverride(time));
	}

	private IEnumerator Look() {
		yield return new WaitForSeconds(timeToStartLooking);
		SetFaceSprite(faces[idx].left);

		yield return new WaitForSeconds(lookDelay);
		SetFaceSprite(faces[idx].right);

		yield return new WaitForSeconds(lookDelay);
		SetFaceSprite(faces[idx].centre);

		lookCoroutine = StartCoroutine(Look());
	}

	private IEnumerator CancelOverride(float time) {
		yield return new WaitForSeconds(time);
		SetFaceSprite(faces[idx].centre);
		lookCoroutine = StartCoroutine(Look());
	}

	private void SetFaceSprite(string id) {
		textureLoader.SetTexture(id);
	}

	[System.Serializable]
	private struct Face {
		public int healthAmount;
		public string centre;
		public string right;
		public string left;
	}
}
