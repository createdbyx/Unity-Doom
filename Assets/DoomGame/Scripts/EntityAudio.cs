using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAudio : MonoBehaviour {

	[SerializeField] private SoundInput[] sounds = null;

	private Dictionary<string, AudioClip[]> soundDict = new Dictionary<string, AudioClip[]>();
	private AudioSource audioSource;

	private void Awake ()
	{
		audioSource = GetComponent<AudioSource> ();
		foreach (SoundInput si in sounds) {
			if (soundDict.ContainsKey (si.name)) {
				Debug.LogError ("Tried to Add Duplicate Sound ID: " + si.name);
				continue;
			} else if (si.soundIds.Length > 0) {
				AudioClip[] clips = new AudioClip[si.soundIds.Length];
				for (int i = 0; i < si.soundIds.Length; i++) {
					clips[i] = SoundLoader.LoadSound(si.soundIds[i]);
				}
				soundDict.Add(si.name, clips);
				continue;
			}
			Debug.LogError("No IDs supplied for Sound: " + si.name);
		}
	}

	public void PlaySound(string soundID) {
		AudioClip[] clips;
		if (soundDict.TryGetValue (soundID, out clips)) {
			audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
			return;
		}
		Debug.LogError ("Tried to play unloaded Sound: " + soundID);
	}

	[System.Serializable]
	private struct SoundInput {
		public string name;
		public string[] soundIds;
	}
}
