using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponLoader : MonoBehaviour {

	private const float AnimationUpdateTick = 0.1f;

	[SerializeField] private string startWeapon = "PISTOL";
	[SerializeField] private RawImage weaponImage = null;
	[SerializeField] private RawImage muzzleImage = null;
	[SerializeField] private AudioSource audioSource = null;
	[SerializeField] private LayerMask entityLayer = 0;
	[SerializeField] private WeaponType[] weaponTypes = null;

	private int weaponIdx = 0;
	private int animIdx = 0;
	private bool playingShotAudio = false;

	// Use this for initialization
	void Start () {
		Invoke("Init", 0); // wait a frame to be sure that the texture loader has initialised
	}

	void Update ()
	{
		// Shoot
		if (Input.GetMouseButtonDown (0)) {
			if (animIdx == 0 && (weaponTypes [weaponIdx].ammo != 0 || weaponTypes [weaponIdx].unlimitedAmmo)) {
				playingShotAudio = false;
				PlayShootAnimation ();
			}
		}

		// Switch Weapons - is there a better way to do this?
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			SetSelectedWeapon(0);
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			SetSelectedWeapon(1);
		}
		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			SetSelectedWeapon(2);
		}
	}

	public void PlayShootAnimation ()
	{
		if (animIdx < weaponTypes [weaponIdx].attackSprites.Length) {
			SetWeaponSprite (weaponTypes [weaponIdx].attackSprites [animIdx]);

			if (!string.IsNullOrEmpty (weaponTypes [weaponIdx].muzzleSprites [animIdx])) {
				// Enable Muzzle
				Texture t = TextureLoader.Instance.GetSpriteTexture (weaponTypes [weaponIdx].muzzleSprites [animIdx]);
				muzzleImage.texture = t;
				muzzleImage.gameObject.SetActive (true);
				((RectTransform)muzzleImage.transform).anchoredPosition = weaponTypes [weaponIdx].muzzlePosition;

				// Play Audio
				if (!playingShotAudio) {
					AudioClip clip = SoundLoader.LoadSound (weaponTypes [weaponIdx].shotSound);
					audioSource.PlayOneShot (clip);
					playingShotAudio = true;
					if (!weaponTypes [weaponIdx].unlimitedAmmo) {
						weaponTypes [weaponIdx].ammo--;
					}
					// Raycast
					GameObject[] hitObjects;
					if (HitEntity (Mathf.Infinity, out hitObjects)) {
						foreach (GameObject go in hitObjects) {
							if (entityLayer.Contains (go.layer)) {
								go.GetComponent<EntityAI>().DamageEntity(weaponTypes [weaponIdx].damage);
							}
						}
					}
				}
			} else {
				muzzleImage.gameObject.SetActive(false);
			}

			animIdx++;
			Invoke("PlayShootAnimation", AnimationUpdateTick);
		} else {
			animIdx = 0;
			SetWeaponSprite (weaponTypes [weaponIdx].idleSprite);
		}
	}

	private void Init() {
		SetSelectedWeapon(startWeapon);
		weaponImage.gameObject.SetActive(true);
	}

	public void ObtainWeapon(string id, WeaponType.State state) {
		for (int i = 0; i < weaponTypes.Length; i++) {
			if (weaponTypes [i].name == id) {
				weaponTypes [i].state = state;
				return;
			}
		}
		Debug.LogError("Invalid Weapon From ID: " + id);
		return;
	}

	public void SetSelectedWeapon(int idx) {
		if (weaponTypes [idx].state == WeaponType.State.Missing) return;
		weaponIdx = idx;
		SetWeaponSprite(weaponTypes [weaponIdx].idleSprite);
	}

	public void SetSelectedWeapon (string id)
	{
		for (int i = 0; i < weaponTypes.Length; i++) {
			if (weaponTypes [i].name == id) {
				SetSelectedWeapon(i);
				return;
			}
		}
		Debug.LogError("Invalid Weapon From ID: " + id);
		return;
	}
	
	private void SetWeaponSprite(string weaponID) {
		Texture t = TextureLoader.Instance.GetSpriteTexture(weaponID);
		weaponImage.texture = t;
	}

	public void SetAmmo (string weaponID, int ammo)
	{
		foreach (WeaponType wt in weaponTypes) {
			if (wt.name == weaponID) {
				wt.ammo = ammo;
				return;
			}
		}
		Debug.LogError("Invalid Weapon From ID: " + weaponID);
	}

	public WeaponType WeaponTypeFromString (string id)
	{
		foreach (WeaponType wt in weaponTypes) {
			if (wt.name == id) {
				return wt;
			}
		}
		Debug.LogError("Invalid Weapon From ID: " + id);
		return new WeaponType();
	}

	private bool HitEntity (float distance, out GameObject[] hitObjects)
	{
		List<GameObject> list = new List<GameObject> ();
		for (int i = 0; i < 10; i++) {
			Vector3 rayOrigin = transform.position + new Vector3 (0, 0.5f * i, 0);
			Ray ray = new Ray (rayOrigin, transform.forward);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, distance, -1)) {
				if (!list.Contains (hit.transform.gameObject)) {
					list.Add (hit.transform.gameObject);
				}
			}
		}

		hitObjects = list.ToArray ();
		return hitObjects.Length > 0;
	}

	[System.Serializable]
	public class WeaponType {
		public string name;
		public int ammo;
		public bool unlimitedAmmo;
		public int damage = 5;
		[TooltipAttribute("ID of sprite when idle")]
		public string idleSprite;
		[TooltipAttribute("ID of Sound Made When Shot")]
		public string shotSound;
		[TooltipAttribute("Position to place muzzle tip")]
		public Vector2 muzzlePosition;
		public AttackType attackType;
		public State state;
		[TooltipAttribute("Sprite IDs to play attack animation")]
		public string[] attackSprites;
		[TooltipAttribute("Sprite ID to play muzzle animation")]
		public string[] muzzleSprites;

		public enum State {
			Normal,
			Missing
		}
	}

	public enum AttackType {
		MeleeOneShot,
		MeleeContinuous,
		RaycastOneShot,
		RaycastContinuous,
		ProjectileOneShot,
		ProjectileContinuous
	}
}
