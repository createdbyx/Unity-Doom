using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Doom {

	public static bool isPaused = false;
	public static bool isLoaded = false;

	public static void LoadScene (string sceneName) {
		UnloadCurrentWad();
		UnloadCurrentMap();
		SceneManager.LoadScene(sceneName);
	}
	public static void LoadScene (int sceneIdx) {
		UnloadCurrentWad();
		UnloadCurrentMap();
		SceneManager.LoadScene(sceneIdx);
	}
	public static void LoadScene (Scene scene) {
		UnloadCurrentWad();
		UnloadCurrentMap();
		SceneManager.LoadScene(scene.buildIndex);
	}

	public static void UnloadCurrentWad ()
	{
		WadLoader.lumps.Clear ();

		if (TextureLoader.Instance != null) {
			TextureLoader.Instance._overrideParameters.Clear ();
			TextureLoader.Instance.OverrideSprites.Clear ();
			TextureLoader.Instance._OverrideSprites.Clear ();
		}
		TextureLoader.PatchNames.Clear();
		TextureLoader.Patches.Clear();
		TextureLoader.MapTextures.Clear();
		TextureLoader.WallTextures.Clear();
		TextureLoader.FlatTextures.Clear();
		TextureLoader.SpriteTextures.Clear();
		TextureLoader.NeedsAlphacut.Clear();
	}

	public static void UnloadCurrentMap() {
		MapLoader.vertices.Clear();
		MapLoader.sectors.Clear();
		MapLoader.linedefs.Clear();
		MapLoader.sidedefs.Clear();
		MapLoader.things.Clear();

		MapLoader.things_lump = null;
		MapLoader.linedefs_lump = null;
		MapLoader.sidedefs_lump = null;
		MapLoader.vertexes_lump = null;
		MapLoader.segs_lump = null;
		MapLoader.ssectors_lump = null;
		MapLoader.nodes_lump = null;
		MapLoader.sectors_lump = null;
		MapLoader.reject_lump = null;
		MapLoader.blockmap_lump = null;

		isLoaded = false;
	}
}
