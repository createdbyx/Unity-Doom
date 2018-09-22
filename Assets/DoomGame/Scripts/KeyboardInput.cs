using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : MonoBehaviour {
	[SerializeField] private PlayerControls controller = null;
	[SerializeField] private WeaponManager weaponManager = null;
	
	// Update is called once per frame
	void Update ()
	{
		if (Doom.isPaused)
			return;

		// Move
		controller.SetInput (new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical")));

		// Use Environment Items
		if (Input.GetKeyDown (KeyCode.Space)) {
			controller.TryUse ();
		}
		
		// Shoot
		if (Input.GetMouseButtonDown (0)) {
			if (controller.Health > 0) {
				weaponManager.Shoot ();
			} else {
				controller.Revive(true);
			}
		}

		// Switch Weapons - is there a better way to do this?
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			weaponManager.SetSelectedWeapon(0);
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			weaponManager.SetSelectedWeapon(1);
		}
		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			weaponManager.SetSelectedWeapon(2);
		}
	}
}
