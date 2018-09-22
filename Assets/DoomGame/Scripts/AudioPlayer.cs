using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour {
	public string audioToPlay;

	private AudioSource audioSource;

	private void Awake() {
		audioSource = GetComponent<AudioSource>();
	}

	public void Go() {
		audioSource.PlayOneShot(SoundLoader.LoadSound(audioToPlay));
	}
}
