using UnityEngine;

public class GUIManager : MonoBehaviour {

	[SerializeField] private GUIFontLetter[] ammoLetters = null;
	[SerializeField] private GUIFontLetter[] healthLetters = null;
	[SerializeField] private GUIFontLetter[] armourLetters = null;

	public void SetAmmo (int ammo)
	{
		ammoLetters [0].gameObject.SetActive (ammo >= 100);
		ammoLetters [1].gameObject.SetActive (ammo >= 10);
		int numberIdx = 0;
		for (int i = 0; i < 3; i++) {
			if (ammoLetters [i].gameObject.activeSelf) {
				ammoLetters[i].SetLetter(ammo.ToString()[numberIdx].ToString());
				numberIdx++;
			}
		}
	}

	public void SetHealth (int health)
	{
		healthLetters[0].gameObject.SetActive(health >= 100);
		healthLetters[1].gameObject.SetActive(health >= 10);
		int numberIdx = 0;
		for (int i = 0; i < 3; i++) {
			if (healthLetters [i].gameObject.activeSelf) {
				healthLetters[i].SetLetter(health.ToString()[numberIdx].ToString());
				numberIdx++;
			}
		}
	}

	public void SetArmour (int armour)
	{
		armourLetters[0].gameObject.SetActive(armour >= 100);
		armourLetters[1].gameObject.SetActive(armour >= 10);
		int numberIdx = 0;
		for (int i = 0; i < 3; i++) {
			if (armourLetters [i].gameObject.activeSelf) {
				armourLetters[i].SetLetter(armour.ToString()[numberIdx].ToString());
				numberIdx++;
			}
		}
	}
}
