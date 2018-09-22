using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerControls : MonoBehaviour 
{
	[Header("Player Stats")]
	[SerializeField] private int health = 100;
	[SerializeField] private int armour = 0;
	[SerializeField] private float speed = 8.0F;
	[SerializeField] private float rotateSpeed = 1.0F;
	[Header("Managers")]
	[SerializeField] private WeaponManager weaponManager = null;
	[SerializeField] private GUIManager guiManager = null;
	[SerializeField] private FaceManager faceManager = null;
	[SerializeField] private GameObject inputManager = null;
	[Header("References")]
    [SerializeField] private AudioSource oofAudio = null;
    [Header("Layers")]
    [SerializeField] private LayerMask pickupLayer = 0;
    [SerializeField] private LayerMask damageLayer = 0;

	CharacterController controller;
	private bool isInDamageZone = false;
	private Coroutine damageCoroutine;
	private Vector3 cameraStartPos;
	private Vector2 input;

	public int Health {
		get {
			return health;
		}
	}

	void Awake() {
		Doom.player = this;
	}

    void Start() {
		Invoke("UpdateGUIStats", 0);
		StartCoroutine(CheckForDamageZone());
		controller = GetComponent<CharacterController> ();
		cameraStartPos = Camera.main.transform.localPosition;
    }

    void Update ()
	{
		if (Doom.isPaused) return;

		if (health == 0) {
			transform.GetChild (0).localPosition = Vector3.MoveTowards (transform.GetChild (0).localPosition, new Vector3 (0, -0.5f, 0), Time.deltaTime);
			return;
		}

		// Move Character
		weaponManager.transform.Rotate (0, input.x * rotateSpeed, 0);
		Vector3 forward = transform.TransformDirection (Vector3.forward);
		float curSpeed = speed * input.y;
		controller.SimpleMove (forward * curSpeed);
		input = Vector2.zero;
    }

    public void SetInput(Vector2 input_) {
    	this.input = input_;
    }

    public void TryUse() {
		Ray ray = new Ray (transform.position, weaponManager.transform.forward);

		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 2, -1)) {
			PokeableLinedef lc = hit.collider.gameObject.GetComponent<PokeableLinedef> ();
			if (lc != null) {
				lc.Poke (gameObject);
			} else {
				oofAudio.PlayOneShot(SoundLoader.LoadSound("DSOOF"));
			}
        }
    }

    public void Revive (bool restartMap)
	{
		if (restartMap) {
			weaponManager.ResetMissionWeapons ();
			Doom.UnloadCurrentMap ();
			WadLoader.Instance.LoadMap ();
		}

		if (health > 0) {
			Debug.LogWarning("Trying to revive when Health > 0");
			return;
		}
		health = 100;
		Camera.main.transform.localPosition = cameraStartPos;
		UpdateGUIStats();
		faceManager.UpdateFace(health);
	}

    public void SetMissionStartWeapons () {
		weaponManager.SetMissionStartWeapons();
	}

    public void TakeDamage (int amount)
	{
		if (health > 0) {
			if (armour > amount) {
				armour -= amount;
			} else {
				armour = 0;
				health -= amount;

				if (health < 0) health = 0;

				guiManager.SetHealth (health);
			}
			guiManager.SetArmour (armour);
			faceManager.UpdateFace(health);
		}
    }

    public void SetInputEnabled(bool enabled) {
		inputManager.SetActive(enabled);
    }

    private void UpdateGUIStats() {
		guiManager.SetAmmo(weaponManager.CurrentAmmo);
		guiManager.SetHealth(health);
		guiManager.SetArmour(armour);
    }

    private void HandlePickup (Pickup p)
	{
		switch (p.pickupType) {
		case Pickup.PickupType.Health:
			if (AddPickupValue (p.strength, p.maxVal, ref health)) {
				p.DoPickup ();
				faceManager.UpdateFace(health);
			}
			break;
		case Pickup.PickupType.Armour:
			if (AddPickupValue (p.strength, p.maxVal, ref armour)) {
				p.DoPickup ();
			}
			break;
		case Pickup.PickupType.Ammo:
			WeaponManager.WeaponType wt = weaponManager.WeaponTypeFromString (p.target);
			int ammo = weaponManager.GetAmmo(wt.ammoType);
			if (p.maxVal == -1) {
				ammo += p.strength;
			} else {
				AddPickupValue (p.strength, p.maxVal, ref ammo);
			}

			weaponManager.SetAmmo(p.target, ammo);
			if (wt.state == WeaponManager.WeaponType.State.Missing && p.toggle) {
				weaponManager.ObtainWeapon(p.target, (WeaponManager.WeaponType.State.Normal));
				weaponManager.SetSelectedWeapon(p.target);
			}
			p.DoPickup();

			break;
		}
		UpdateGUIStats();
    }

    private bool AddPickupValue (int addVal, int maxVal, ref int outVal)
	{
		if (outVal < maxVal) {
			if (maxVal - addVal < outVal) {
				if (outVal < maxVal) {
					outVal = maxVal;
					return true;
				}
			} else {
				outVal += addVal;
				return true;
			}
		}
		return false;
	}

	private IEnumerator ZoneDamage (float updateRate, int damage)
	{
		yield return new WaitForSeconds (updateRate);
		if (isInDamageZone) {
			TakeDamage(damage);
			damageCoroutine = StartCoroutine(ZoneDamage(1, 5));
		}
	}

    void OnTriggerEnter (Collider coll)
	{
		if (pickupLayer.Contains (coll.gameObject.layer)) {
			Pickup p = coll.GetComponent<Pickup>();
			HandlePickup(p);
		}
	}

	private IEnumerator CheckForDamageZone ()
	{
		yield return new WaitForSeconds (0.1f);
		RaycastHit hit;
		if (Physics.Raycast (transform.position, -transform.up, out hit, 1, damageLayer)) {
			if (!isInDamageZone) {
				isInDamageZone = true;
				damageCoroutine = StartCoroutine (ZoneDamage (0.5f, 2));
			}
		} else if (isInDamageZone) {
			isInDamageZone = false;
			StopCoroutine(damageCoroutine);
		}
		StartCoroutine(CheckForDamageZone());
	}
}
