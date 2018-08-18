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
		StartCoroutine(SetLetterWait(letter, fontType, 0.1f));
	}

	private IEnumerator SetLetterWait (string letter_, FontLoader.FontType fontType_, float wait) {
		yield return new WaitForSeconds(wait);
		SetLetter(letter_, fontType_);
	}

	public void SetLetter (string letter_, FontLoader.FontType fontType_) {
		rawImage.texture = FontLoader.GetTexture(letter_, fontType_);
		rawImage.SetNativeSize();
		letter = letter_;
		fontType = fontType_;
	}

	public void SetLetter (string letter_) {
		rawImage.texture = FontLoader.GetTexture(letter_, fontType);
		rawImage.SetNativeSize();
		letter = letter_;
	}
}
