using UnityEngine;
using UnityEngine.UI;
using System.Collections;
[RequireComponent(typeof(RawImage))]
public class GUIFontLetter : MonoBehaviour {

	[SerializeField] private string letter = "";
	[SerializeField] private FontLoader.FontType fontType = FontLoader.FontType.BigRed;
	[SerializeField] private bool loadOnStart = false;

	private RawImage rawImage;

	// Use this for initialization
	void Start () {
		rawImage = GetComponent<RawImage>();
		if (loadOnStart)
		Invoke("Initialise", 0);
	}

	private void Initialise () {
		SetLetter(letter, fontType);
		rawImage.color = Color.white;
	}

	public void SetLetter (string letter_, FontLoader.FontType fontType_) {
		rawImage.texture = FontLoader.GetTexture(letter_, fontType_);
		rawImage.SetNativeSize();
		letter = letter_;
		fontType = fontType_;
		rawImage.color = Color.white;
	}

	public void SetLetter (string letter_) {
		rawImage.texture = FontLoader.GetTexture(letter_, fontType);
		rawImage.SetNativeSize();
		letter = letter_;
		rawImage.color = Color.white;
	}
}
