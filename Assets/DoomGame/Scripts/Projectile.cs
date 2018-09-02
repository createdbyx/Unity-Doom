using UnityEngine;

public class Projectile : MonoBehaviour {

	[SerializeField] private float speed = 1;
	[SerializeField] private float maxLifetime = 30;
	[SerializeField] private int damage = 1;

	private int hitLayer;

	// Use this for initialization
	void Start ()
	{
		hitLayer = LayerMask.NameToLayer("Entity");
		if (maxLifetime > 0) {
			Invoke("DestroySelf", maxLifetime);
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += transform.forward * speed * Time.deltaTime;
	}

	private void DestroySelf() {
		GameObject.Destroy(gameObject);
	}

	private void OnCollisionEnter (Collision col)
	{
		if (col.gameObject.layer == hitLayer) {
			EntityAI ai = col.gameObject.GetComponent<EntityAI> ();
			if (ai == null) {
				PlayerControls player = col.gameObject.GetComponent<PlayerControls> ();
				if (player == null) {
					Debug.Log ("Invalid Object set to Entity Layer: " + col.gameObject.name);
				}
				player.TakeDamage (damage);
			} else {
				ai.DamageEntity(damage);
			}
		}
		DestroySelf();
	}
}
