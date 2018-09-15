using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour {

	private const float AnimationUpdateTick = 0.1f;

	[SerializeField] private string startWeapon = "PISTOL";
	[SerializeField] private GUIManager guiManager = null;
	[SerializeField] private AudioSource audioSource = null;
	[SerializeField] private LayerMask entityLayer = 0;
	[SerializeField] private LayerMask explosiveLayer = 0;
	[SerializeField] private AmmoConfig[] ammo = null;
	[SerializeField] private WeaponType[] weaponTypes = null;

	private bool canShoot = true;
	private Dictionary<AmmoType, AmmoConfig> ammoDict = new Dictionary<AmmoType, AmmoConfig>();

	private int weaponIdx = 0;
	private int animIdx = 0;

	private WeaponType.State[] missionStartWeapons;
	private int[] missionStartAmmo;

	public int CurrentAmmo {
		get {
			return (weaponTypes[weaponIdx].ammoType == AmmoType.UNLM ? -1 : GetAmmo(weaponTypes[weaponIdx].name));
		}
	}

	// Use this for initialization
	void Start () {
		Invoke("InitAmmo", 0); // wait a frame to be sure that the texture loader has initialised
		Invoke("InitWeapon", 0); // wait a frame to be sure that the texture loader has initialised
	}

	public int GetAmmo (AmmoType ammoType)
	{
		AmmoConfig conf;
		if (ammoDict.TryGetValue (ammoType, out conf)) {
			return conf.currentAmmo;
		}
		Debug.LogError("Ammo Type Not Listed: " + ammoType.ToString());
		return 0;
	}

	public void Shoot () {
		if (canShoot)
		if (animIdx == 0 && (GetAmmo(weaponTypes[weaponIdx].name) != 0 || weaponTypes [weaponIdx].ammoType == AmmoType.UNLM)) {
			PlayShootAnimation ();
		}
	}

	public void SetMissionStartWeapons ()
	{
		missionStartWeapons = new WeaponType.State[weaponTypes.Length];
		for (int i = 0; i < missionStartWeapons.Length; i++) {
			missionStartWeapons[i] = weaponTypes[i].state;
		}

		missionStartAmmo = new int[ammo.Length];
		for (int i = 0; i < missionStartAmmo.Length; i++) {
			missionStartAmmo[i] = ammo[i].currentAmmo;
		}

	}

	public void ResetMissionWeapons () {
		for (int i = 0; i < missionStartWeapons.Length; i++) {
			weaponTypes[i].state = missionStartWeapons[i];
		}
		for (int i = 0; i < missionStartAmmo.Length; i++) {
			ammo[i].currentAmmo = missionStartAmmo[i];
		}
	}

	public void PlayShootAnimation ()
	{
		if (animIdx < weaponTypes [weaponIdx].attackSprites.Length) {
			SetWeaponSprite (weaponTypes [weaponIdx].attackSprites [animIdx]);

			// Muzzle
			if (animIdx < weaponTypes [weaponIdx].muzzleSprites.Length && !string.IsNullOrEmpty (weaponTypes [weaponIdx].muzzleSprites [animIdx])) {
				Texture t = TextureLoader.Instance.GetSpriteTexture (weaponTypes [weaponIdx].muzzleSprites [animIdx]);
				weaponTypes [weaponIdx].muzzleImage.texture = t;
				weaponTypes [weaponIdx].muzzleImage.gameObject.SetActive (true);
			} else if (weaponTypes[weaponIdx].muzzleImage != null) {
				weaponTypes [weaponIdx].muzzleImage.gameObject.SetActive (false);
			}

			// Audio
			if (animIdx < weaponTypes [weaponIdx].audioFrames.Length) {
				if (!string.IsNullOrEmpty (weaponTypes [weaponIdx].audioFrames [animIdx])) {
					AudioClip clip = SoundLoader.LoadSound (weaponTypes [weaponIdx].audioFrames [animIdx]);
					audioSource.PlayOneShot (clip);
				}
			}

			if (animIdx == weaponTypes [weaponIdx].attackIdx) {
				if (weaponTypes [weaponIdx].ammoType != AmmoType.UNLM) {
					SetAmmo(weaponTypes[weaponIdx].name, GetAmmo(weaponTypes[weaponIdx].name) - 1);
					guiManager.SetAmmo(GetAmmo(weaponTypes[weaponIdx].name));
				}
				switch (weaponTypes[weaponIdx].attackType) {
				case AttackType.MeleeOneShot:
					TryHitRaycast(1.5f, weaponTypes[weaponIdx].damage);
					break;
				case AttackType.RaycastOneShot:
					TryHitRaycast(Mathf.Infinity, weaponTypes[weaponIdx].damage);
					break;
				}
			}

			animIdx++;
			Invoke("PlayShootAnimation", AnimationUpdateTick);
		} else {
			animIdx = 0;
			SetWeaponSprite (weaponTypes [weaponIdx].idleSprite);
		}
	}

	private void InitAmmo ()
	{
		foreach (AmmoConfig ac in ammo) {
			if (!ammoDict.ContainsKey (ac.ammoType)) {
				ac.UpdateAmmoDisplay();
				ammoDict.Add(ac.ammoType, ac);
				continue;
			}
			Debug.LogError("Already contains definition for AmmoType: " + ac.ammoType.ToString());
		}
	}

	private void InitWeapon ()
	{
		// Set curent selection in Weapon Manager
		SetSelectedWeapon (startWeapon);

		// check weapon sets and update arms display
		foreach (WeaponType wt in weaponTypes) {
			wt.UpdateArmsDisplay();
		}

		SetMissionStartWeapons();
	}

	public void ObtainWeapon(string id, WeaponType.State state) {
		for (int i = 0; i < weaponTypes.Length; i++) {
			if (weaponTypes [i].name == id) {
				weaponTypes [i].state = state;
				weaponTypes [i].UpdateArmsDisplay();
				return;
			}
		}
		Debug.LogError("Invalid Weapon From ID: " + id);
		return;
	}

	public void SetSelectedWeapon(int idx) {
		if (idx == weaponIdx) return;
		if (!canShoot || weaponTypes [idx].state == WeaponType.State.Missing) return;

		weaponTypes[weaponIdx].weaponAnimator.SetBool("up", false);
		weaponIdx = idx;
		canShoot = false;
		Invoke("GetNextWeapon", 0.5f);
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

	private void GetNextWeapon() {
		SetWeaponSprite(weaponTypes [weaponIdx].idleSprite);
		weaponTypes[weaponIdx].weaponAnimator.SetBool("up", true);
		guiManager.SetAmmo(GetAmmo(weaponTypes[weaponIdx].name));
		canShoot = true;
	}
	
	private void SetWeaponSprite(string weaponID) {
		Texture t = TextureLoader.Instance.GetSpriteTexture(weaponID);
		weaponTypes[weaponIdx].weaponImage.texture = t;
	}

	private int GetAmmo (string weaponID)
	{
		foreach (WeaponType wt in weaponTypes) {
			if (wt.name == weaponID) {
				AmmoConfig outConf;
				if (ammoDict.TryGetValue (wt.ammoType, out outConf)) {
					return outConf.currentAmmo;
				}
				if (wt.ammoType != AmmoType.UNLM) {
					Debug.LogError ("Weapon: " + wt.name + " using unlisted ammo type: " + wt.ammoType.ToString ());
					return 0;
				}
				return -1;
			}
		}
		Debug.LogError("Invalid Weapon ID: " + weaponID);
		return 0;
	}

	public void SetAmmo (string weaponID, int ammo)
	{
		foreach (WeaponType wt in weaponTypes) {
			if (wt.name == weaponID) {
				//wt.ammo = ammo;
				AmmoConfig outConf;
				if (ammoDict.TryGetValue (wt.ammoType, out outConf)) {
					outConf.currentAmmo = ammo;
					outConf.UpdateAmmoDisplay();
					return;
				}
				Debug.LogError ("Weapon: " + wt.name + " using unlisted ammo type: " + wt.ammoType.ToString ());
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

	private void TryHitRaycast (float distance, int damage)
	{
		GameObject[] hitObjects;
		if (HitEntity (distance, out hitObjects)) {
			foreach (GameObject go in hitObjects) {
				if (go == gameObject) continue;

				if (entityLayer.Contains (go.layer)) {
					try {
						go.GetComponent<EntityAI> ().DamageEntity (damage);
					} catch {
						Debug.LogError ("Invalid object with Entity layer: " + go.name);
					}
					return;
				}
				if (explosiveLayer.Contains (go.layer)) {
					try {
						go.GetComponent<ExplosionObject> ().TakeDamage (damage);
					} catch {
						Debug.LogError ("Invalid object with Explosive layer: " + go.name);
					}
				}
			}
		}
	}

	private bool HitEntity (float distance, out GameObject[] hitObjects)
	{
		List<GameObject> list = new List<GameObject> ();
		for (int i = 0; i < 10; i++) {
			Vector3 rayOrigin = transform.position + new Vector3 (0, (0.5f * i)-1, 0);
			Ray ray = new Ray (rayOrigin, transform.forward);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, distance)) {
				if (!list.Contains (hit.transform.gameObject)) {
					list.Add (hit.transform.gameObject);
				}
			}
		}

		hitObjects = list.ToArray ();
		return hitObjects.Length > 0;
	}

	[System.Serializable]
	public class AmmoConfig {
		public AmmoType ammoType;
		public int currentAmmo;
		public int maxAmmo;
		public GUIFontLetter[] ammoDisplay;
		public GUIFontLetter[] maxDisplay;

		public void UpdateAmmoDisplay() {
			ammoDisplay [0].gameObject.SetActive (currentAmmo >= 100);
			ammoDisplay [1].gameObject.SetActive (currentAmmo >= 10);
			int numberIdx = 0;
			for (int i = 0; i < 3; i++) {
				if (ammoDisplay [i].gameObject.activeSelf) {
					ammoDisplay[i].SetLetter(currentAmmo.ToString()[numberIdx].ToString());
					numberIdx++;
				}
			}
			maxDisplay [0].gameObject.SetActive (maxAmmo >= 100);
			maxDisplay [1].gameObject.SetActive (maxAmmo >= 10);
			numberIdx = 0;
			for (int i = 0; i < 3; i++) {
				if (maxDisplay [i].gameObject.activeSelf) {
					maxDisplay[i].SetLetter(maxAmmo.ToString()[numberIdx].ToString());
					numberIdx++;
				}
			}
		}
	}

	public enum AmmoType {
		BULL,
		SHEL,
		RCKT,
		CELL,
		UNLM
	}

	[System.Serializable]
	public class WeaponType {
		public string name;
		public AmmoType ammoType;
		//public bool unlimitedAmmo;
		public int damage = 5;
		[TooltipAttribute("ID of sprite when idle")]
		public string idleSprite;
		public AttackType attackType;
		public State state;
		[SerializeField] private Transform armsDisplay;
		public Animator weaponAnimator;
		public RawImage weaponImage;
		public RawImage muzzleImage;
		[TooltipAttribute("Sprite IDs to play attack animation")]
		public string[] attackSprites;
		[TooltipAttribute("Sprite IDs to play muzzle animation")]
		public string[] muzzleSprites;
		[TooltipAttribute("Audio IDs to play at animation frames")]
		public string[] audioFrames;
		[TooltipAttribute("Animation idx to start weapon attack logic (raycasts etc)")]
		public int attackIdx;

		public enum State {
			Normal,
			Missing
		}

		public void UpdateArmsDisplay ()
		{
			if (armsDisplay != null) {
				armsDisplay.GetChild(0).gameObject.SetActive(state != State.Missing);
				armsDisplay.GetChild(1).gameObject.SetActive(state == State.Missing);
			}
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
