using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(EntityAnimator))]
public class EntityAI : MonoBehaviour {

	private const float AIUpdateRate = 0.1f;
	private const float targetDistance = 50;

	[SerializeField] private AIStrategy aiStrategy = AIStrategy.GuardPoint;
	[SerializeField] private float moveSpeed = 5;
	[SerializeField] private float attackDistance = 5;
	[SerializeField] private int health = 10;
	[SerializeField] private int attackDamage = 1;
	[SerializeField] private string deadLayer = "";

	private EntityAnimator animator;
	private bool isMoving = false;
	private bool isDead = false;
	private bool movementOverride = false;

	// Use this for initialization
	void Start () {
		animator = GetComponent<EntityAnimator>();
		StartCoroutine(UpdateAI());
	}

	void Update ()
	{
		if (!isDead && isMoving && !movementOverride) {
			transform.position = Vector3.MoveTowards (transform.position, transform.position + transform.forward, moveSpeed * Time.deltaTime);
		}
	}

	public void DoRaycastHit ()
	{
		Vector3 targetPos = Camera.main.transform.position;
		targetPos.y = transform.position.y;
		transform.LookAt(targetPos);
		Debug.DrawLine(transform.position + transform.up * 1, transform.position + transform.up * 1 + (transform.forward * 15), Color.red, 0.2f);

		int layer = LayerMask.NameToLayer("Entity");
		Debug.Log(LayerMask.LayerToName(layer));


		RaycastHit hit;
		if (Physics.Raycast (transform.position + transform.forward, transform.forward, out hit, Mathf.Infinity, 1 << layer)) {
			EntityAI ai = hit.transform.GetComponent<EntityAI> ();
			if (ai == null) {
				PlayerControls player = hit.transform.GetComponent<PlayerControls> ();
				if (player == null) {
					Debug.Log ("Invalid Object set to Entity Layer: " + hit.transform.name);
				}
				player.TakeDamage (attackDamage);
			} else {
				ai.DamageEntity(attackDamage);
			}
		}
		targetPos.y = Camera.main.transform.position.y;
		transform.LookAt(targetPos);
	}

	public void FireProjectile(GameObject prefab) {
		GameObject go = GameObject.Instantiate(prefab, transform.position + transform.forward + (transform.up * 0.5f), Quaternion.identity);
		go.transform.LookAt(Camera.main.transform.position);
	}

	public void StopMovement() {
		movementOverride = true;
		isMoving = false;
	}

	public void ResumeMovement() {
		movementOverride = false;
	}

	public void DamageEntity (int amount)
	{
		health -= amount;
		if (health <= 0) {
			isDead = true;
			animator.SetAnimationSet ("DEAD");
			animator.StopAnimationUpdateAfterCurrent = true;
			gameObject.layer = LayerMask.NameToLayer (deadLayer);
		} else {
			animator.SetAnimationSet("HIT");
		}
	}
	
	private IEnumerator UpdateAI ()
	{
		yield return new WaitForSeconds (AIUpdateRate);
		if (!isDead) {
			if (!movementOverride) {
				Vector3 targetPos = Camera.main.transform.position;
				targetPos.y = transform.position.y;
				// TODO: Raycast to find if we can actually see player
				if (Vector3.Distance (transform.position, targetPos) < attackDistance) {
					transform.LookAt (targetPos);
					animator.SetAnimationSet ("ATTACK");
					isMoving = false;
				} else {
					switch (aiStrategy) {
						case AIStrategy.GuardPoint:
							animator.SetAnimationSet ("IDLE");
						break;
						case AIStrategy.FollowPlayer:
							if (Vector3.Distance (transform.position, targetPos) < targetDistance) {
								transform.LookAt (targetPos);
								animator.SetAnimationSet ("MOVE");
								isMoving = true;
							}
						break;
					}
				}
			}

			StartCoroutine (UpdateAI ());
		}
	}

	private enum AIStrategy {
		GuardPoint,
		FollowPlayer
	}
}
