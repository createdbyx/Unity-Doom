using UnityEngine;
using UnityEngine.UI;
using System.Collections;
[RequireComponent(typeof(RawImage))]
public class GUITextureLoader : MonoBehaviour {

	[SerializeField] private string textureId = null;
	private RawImage rawImage;

	// Use this for initialization
	void Start () {
		rawImage = GetComponent<RawImage>();
		StartCoroutine(SetTextureWait(textureId));
	}

	private IEnumerator SetTextureWait (string id) {
		yield return new WaitForEndOfFrame();
		SetTexture(id);
	}

	public void SetTexture (string id)
	{
		if (WadLoader.lumps.Count == 0)
        {
            Debug.LogError("MapLoader: Load: WadLoader.lumps == 0");
            return;
        }

		foreach (Lump l in WadLoader.lumps) {
			if (l.lumpName == id) {
				rawImage.texture = TextureLoader.Instance.TextureFromLump (l);
				return;
			}
		}
		Debug.LogError("Invalid id: " + id);
	}
}
