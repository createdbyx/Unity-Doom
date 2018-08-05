using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

	private AudioSource audioSource;

	void Start () {
		audioSource = GetComponent<AudioSource>();
		StartCoroutine(PlayMusic("D_E1M1", 0.5f));
	}
	
	public void PlayMusic(string musicID) {
		StartCoroutine(PlayMusic(musicID, 0));
	}

	private IEnumerator PlayMusic (string musicID, float wait) {
		yield return new WaitForSeconds(wait); // we need a small wait for the SoundLoader to initialise first time
		audioSource.PlayOneShot(SoundLoader.LoadSound(musicID));

		Debug.LogWarning("TODO: Fix Format Issue. Defaulting to temporary solution...");
		audioSource.Play();
	}
}
