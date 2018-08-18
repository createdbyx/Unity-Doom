using UnityEngine;

public class GUIManager : MonoBehaviour {

	[SerializeField] private GUIFontLetter[] ammoLetters = null;
	[SerializeField] private GUIFontLetter[] healthLetters = null;
	[SerializeField] private GUIFontLetter[] armourLetters = null;

	public void SetAmmo (int ammo)
	{
		string asString = "";
		if (ammo < 100) {
			asString = "0";
		}
		if (ammo < 10) {
			asString += "0";
		}
		asString += ammo.ToString ();
		for (int i = 0; i < 3; i++) {
			ammoLetters[i].SetLetter(asString[i].ToString());
		}
	}

	public void SetHealth (int health)
	{
		string asString = "";
		if (health < 100) {
			asString = "0";
		}
		if (health < 10) {
			asString += "0";
		}
		asString += health.ToString ();
		for (int i = 0; i < 3; i++) {
			healthLetters[i].SetLetter(asString[i].ToString());
		}
	}

	public void SetArmour (int armour)
	{
		string asString = "";
		if (armour < 100) {
			asString = "0";
		}
		if (armour < 10) {
			asString += "0";
		}
		asString += armour.ToString ();
		for (int i = 0; i < 3; i++) {
			armourLetters[i].SetLetter(asString[i].ToString());
		}
	}
}
