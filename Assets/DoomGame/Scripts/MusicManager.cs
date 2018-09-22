using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

	[Header("TEMPORARY SOLUTION")]
	[SerializeField] private AudioClip D_INTROA = null;
	[SerializeField] private AudioClip D_INTER = null;
	[SerializeField] private AudioClip D_E1M1 = null;

	private AudioSource audioSource;

	void Start () {
		audioSource = GetComponent<AudioSource>();
		StartCoroutine(PlayMusic("D_INTROA", 0.5f));
	}
	
	public void PlayMusic(string musicID) {
		StartCoroutine(PlayMusic(musicID, 0));
	}

	private IEnumerator PlayMusic (string musicID, float wait) {
		yield return new WaitForSeconds(wait); // we need a small wait for the SoundLoader to initialise first time
		audioSource.PlayOneShot(SoundLoader.LoadSound(musicID));

		Debug.LogWarning("TODO: Fix Format Issue. Defaulting to temporary solution...");
		switch (musicID) {
		case "D_INTROA":
			audioSource.clip = D_INTROA;
			audioSource.Play();
			break;
		case "D_INTER":
			audioSource.clip = D_INTER;
			audioSource.Play();
			break;
		case "D_E1M1":
			audioSource.clip = D_E1M1;
			audioSource.Play();
			break;
		}
	}
}
