using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Pickup : MonoBehaviour {

	public PickupType pickupType;
	public string target;
	public int strength;
	public int maxVal;
	public bool toggle;

	public string pickupSound = "DSITEMUP";

	private AudioClip pickupClip;

	// Use this for initialization
	void Start () {
		pickupClip = SoundLoader.LoadSound(pickupSound);
	}
	
	public void DoPickup() {
		GameObject go = new GameObject("AudioPlayer");
		go.AddComponent<AudioSource>().PlayOneShot(pickupClip);
		go.AddComponent<DestroyItem>().DestroyObject(pickupClip.length);
		Destroy(gameObject);
	}

	public enum PickupType {
		Health,
		Armour,
		Ammo
	}
}
#if UNITY_EDITOR
[CustomEditor(typeof(Pickup))]
public class PickupEditor : Editor 
{
    public override void OnInspectorGUI ()
	{
		Pickup main = (Pickup)target;

		main.pickupType = (Pickup.PickupType)EditorGUILayout.EnumPopup ("Pickup Type", main.pickupType);

		switch (main.pickupType) {
			case Pickup.PickupType.Health:
			main.strength = EditorGUILayout.IntField("Health", main.strength);
			main.maxVal = EditorGUILayout.IntField("Max Health", main.maxVal);
			break;
			case Pickup.PickupType.Armour:
			main.strength = EditorGUILayout.IntField("Armour", main.strength);
			main.maxVal = EditorGUILayout.IntField("Max Armour", main.maxVal);
			break;
			case Pickup.PickupType.Ammo:
			main.target = EditorGUILayout.TextField("Weapon", main.target);
			main.strength = EditorGUILayout.IntField("Ammo", main.strength);
			main.maxVal = EditorGUILayout.IntField("Max Ammo", main.maxVal);
			main.toggle = EditorGUILayout.Toggle("Collect Missing Weapon", main.toggle);
			break;
		}

		main.pickupSound = EditorGUILayout.TextField("Pickup Sound", main.pickupSound);
		EditorUtility.SetDirty(main);
    }
}
#endif