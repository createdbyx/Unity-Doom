using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionObject : MonoBehaviour {

	private const float AnimationUpdateRate = 0.1f;

	[SerializeField] private int health = 1;
	[SerializeField] private int damage = 5;
	[SerializeField] private float radius = 2;
	[SerializeField] private string[] frames = null;
	[SerializeField] private Vector2[] scales; // this is one of those fun animations that need scaling for each frame
	[SerializeField] private int damageFrame = 0;
	[SerializeField] private LayerMask entityLayer = 0;

	private ThingController thingController;
	private TextureAnimation textureAnimation; // we record this to destroy it as there's no nice way for us to call an event from TextureAnimation and I don't really want to devote this script to idle animation - it's ok if this doesn't exist on an object
	private int animationFrame;

	// Use this for initialization
	void Start () {
		thingController = GetComponent<ThingController>();
		textureAnimation = GetComponent<TextureAnimation>();
	}

	public void TakeDamage (int amount)
	{
		if (health <= 0) return;
		health -= amount;
		if (health <= 0) {
			if (textureAnimation != null) {
				Destroy(textureAnimation);
			}
			StartCoroutine(UpdateAnimationFrame());
		}
	}

	private void DamageNearbyEntities ()
	{
		Collider[] hits = Physics.OverlapSphere(transform.position, radius, entityLayer);
		for (int i = 0; i < hits.Length; i++) {
			EntityAI ai = hits[i].transform.GetComponent<EntityAI> ();
			if (ai == null) {
				PlayerControls player = hits[i].transform.GetComponent<PlayerControls> ();
				if (player == null) {
					Debug.LogError("Invalid Object on Entity layer: " + hits[i].transform.name);
					return;
				}
				player.TakeDamage(damage);
				return;
			}
			ai.DamageEntity(damage);
		}
	}

	private IEnumerator UpdateAnimationFrame ()
	{
		yield return new WaitForSeconds (AnimationUpdateRate);
		if (animationFrame == damageFrame) {
			DamageNearbyEntities ();
		}
		if (animationFrame < frames.Length) {
			thingController.SetTexture (TextureLoader.Instance.GetSpriteTexture (frames [animationFrame]));

			if (animationFrame < scales.Length) {
				thingController.SetWidth (scales [animationFrame].x);
				thingController.SetHeight (scales [animationFrame].y);
			}

			animationFrame++;
			StartCoroutine (UpdateAnimationFrame ());
			
		} else {
			Destroy(gameObject);
		}
	}

	#if UNITY_EDITOR
	void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    #endif
}
