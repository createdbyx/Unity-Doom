using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

	[SerializeField] private WadLoader wadLoader = null;
	[SerializeField] private GameObject startCamera = null;
	[Header("New Game")]
	[SerializeField] private GameObject startPanel = null;
	[SerializeField] private GameObject mapSelectPanel = null;
	[SerializeField] private GameObject difficultyPanel = null;

	[Header("Game Parts")]
	[SerializeField] private GameObject player = null;
	[SerializeField] private GameObject gameCanvas = null;

	void Update () {
		if (Doom.isLoaded && Input.GetKeyDown (KeyCode.Escape)) {
			Doom.isPaused = !Doom.isPaused;
			Time.timeScale = Doom.isPaused ? 0 : 1;
			startPanel.SetActive(Doom.isPaused);
		}
	}

	public void PauseGame() {
		Reset();
		Time.timeScale = 0;
	}

	public void Reset () {
		startPanel.SetActive(true);
		mapSelectPanel.SetActive(false);
		difficultyPanel.SetActive(false);
	}

	public void NewGame () {
		startPanel.SetActive(false);
		mapSelectPanel.SetActive(true);
		difficultyPanel.SetActive(false);
	}

	public void SelectEpisode(int episode) {
		wadLoader.SetAutoLoadEpisode(episode);
		mapSelectPanel.SetActive(false);
		difficultyPanel.SetActive(true);
	}

	// TODO: give this some actual effect
	public void SelectDifficulty() {
		Doom.isPaused = false;
        Time.timeScale = 1;
		wadLoader.LoadMap();
		startCamera.SetActive(false);
		difficultyPanel.SetActive(false);
		player.SetActive(true);
		gameCanvas.SetActive(true);
	}
}
