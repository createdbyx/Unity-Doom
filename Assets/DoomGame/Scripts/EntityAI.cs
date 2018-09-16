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
	[SerializeField] private float timeBetweenShots = 2;
	[SerializeField] private string deadLayer = "";

	private EntityAnimator animator;
	private bool isMoving = false;
	private bool isDead = false;
	private bool movementOverride = false;
	private bool canSeePlayer = false;
	private float shootTimer;
	private bool isWandering = false;

	// Use this for initialization
	void Start () {
		shootTimer = timeBetweenShots;
		animator = GetComponent<EntityAnimator>();
		StartCoroutine(UpdateAI());
	}

	void Update ()
	{
		if (isDead) return;

		// Movement controls for strats
		switch (aiStrategy) {
		case AIStrategy.FollowPlayer:
			if (isWandering) { // move straight forward to wander in previously set direction
				transform.position += transform.forward * moveSpeed / 2 * Time.deltaTime;
			} else if (isMoving && !movementOverride && canSeePlayer) {
				// move towards previously set target destination
				transform.position = Vector3.MoveTowards (transform.position, transform.position + transform.forward, moveSpeed * Time.deltaTime);
			}
			break;
		case AIStrategy.GuardPoint:
			if (isWandering) {
				transform.LookAt (Camera.main.transform.position);
			}
			break;
		}

		// Increment Shoot Timer
		if (isWandering) {
			shootTimer += Time.deltaTime;
			if (shootTimer >= timeBetweenShots) {
				isWandering = false;
			}
		}
	}

	public void DoRaycastHit ()
	{
		PlayerControls player;
		if (CanSeeEntity (out player)) {
			player.TakeDamage (attackDamage);
		} else {
			EntityAI ai;
			if (CanSeeEntity (out ai)) {
				ai.DamageEntity(attackDamage);
			}
		}
	}

	public void FireProjectile(GameObject prefab) {
		GameObject go = GameObject.Instantiate(prefab, transform.position + transform.forward + (transform.up * 0.5f), Quaternion.identity);
		go.transform.LookAt(Camera.main.transform.position);
		go.GetComponent<Projectile>().AddIgnoreObject(gameObject);
	}

	public void StopMovement() {
		movementOverride = true;
		isMoving = false;
	}

	public void ResumeMovement ()
	{
		movementOverride = false;
		// Prepare for shoot delay movement
		shootTimer = 0;
		Invoke("StartWander", 0.1f); // we invoke this otherwise we miss the last frame of animation
	}

	private void StartWander ()
	{
		if (aiStrategy != AIStrategy.GuardPoint) {
			animator.SetAnimationSet ("MOVE");
			transform.eulerAngles = new Vector3 (transform.eulerAngles.x, Random.Range (0, 360), transform.eulerAngles.z);
		} else if (health > 0) {
			animator.SetAnimationSet ("IDLE");
		} else {
			animator.SetAnimationSet ("DEAD");
		}
		isWandering = true;
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

	// Raycast for player
	private bool CanSeeEntity (out PlayerControls entity) {
		GameObject go;
		if (EntityRaycast (out go)) {
			entity = go.GetComponent<PlayerControls> ();
			if (entity != null) {
				return true;
			}
		}
		entity = null;
		return false;
	}

	// raycast for enemy
	private bool CanSeeEntity (out EntityAI entity) {
		GameObject go;
		if (EntityRaycast (out go)) {
			entity = go.GetComponent<EntityAI> ();
			if (entity != null) {
				return true;
			}
		}
		entity = null;
		return false;
	}

	// General raycast for Enemies & player returning a GameObject
	private bool EntityRaycast (out GameObject go)
	{
		go = null;
		RaycastHit hit;
		if (Physics.Raycast (transform.position + transform.up, transform.forward, out hit, Mathf.Infinity)) {
			if (hit.transform.gameObject.layer == gameObject.layer) {
				go = hit.transform.gameObject;
			}
			#if UNITY_EDITOR
			Debug.DrawLine (transform.position + transform.up, hit.point, Color.red, 0.2f);
			#endif
		}
		#if UNITY_EDITOR
		else {
			Debug.DrawLine (transform.position + transform.up, transform.position + transform.up + (transform.forward * 15), Color.red, 0.2f);
		}
		#endif

		return go != null;
	}
	
	private IEnumerator UpdateAI ()
	{
		yield return new WaitForSeconds (AIUpdateRate);
		if (!isDead) {
			if (!movementOverride && !isWandering) {

				Vector3 originalDir = transform.forward;
				Vector3 targetPos = Camera.main.transform.position;
				targetPos.y = transform.position.y;

				// Attack player if within distance, otherwise see what AI strat says
				if (canSeePlayer && Vector3.Distance (transform.position, targetPos) < attackDistance) {
					AI_Attack (targetPos);
				} else {
					AI_Switch (targetPos);
				}

				if (!canSeePlayer) {
					// look back to original position
					transform.LookAt (originalDir);
				}
			}

			StartCoroutine (UpdateAI ());
		}
	}

	private void AI_Switch (Vector3 targetPos)
	{
		switch (aiStrategy) {
		case AIStrategy.GuardPoint:
			AI_Idle();
			break;
		case AIStrategy.FollowPlayer:
			AI_FollowPlayer(targetPos);
			break;
		}
	}

	private void AI_Idle () {
		animator.SetAnimationSet ("IDLE");
		AI_LookForPlayer();
	}

	private void AI_FollowPlayer (Vector3 targetPos)
	{
		// raycast around to see if we can find the player
		if (!canSeePlayer) {
			AI_LookForPlayer();
		}

		if (canSeePlayer) { // Move towards a found player
			if (Vector3.Distance (transform.position, targetPos) < targetDistance) {
				transform.LookAt (targetPos);
				animator.SetAnimationSet ("MOVE");
				isMoving = true;
				canSeePlayer = true;
			} else { // too far away - stay idle
				AI_Idle();
			}
		}
	}

	private void AI_Attack(Vector3 targetPos) {

		PlayerControls p; // raycast ahead to see if we can shoot
		if (CanSeeEntity (out p)) {
			transform.LookAt (targetPos);
			animator.SetAnimationSet ("ATTACK");
			isMoving = false;
			canSeePlayer = true;
		} else if (canSeePlayer) { // we can't shoot, revert back to AI start
			AI_Switch(targetPos);
		}
	}

	// Raycast towards player to see if in line of sight and start attacking
	private void AI_LookForPlayer ()
	{
		// Don't look if beyond range
		if (Vector3.Distance (transform.position, Camera.main.transform.position) > targetDistance) {
			canSeePlayer = false;
			return;
		}

		PlayerControls p;
		for (int i = 0; i < 360; i += 10) {
			Vector3 point = Camera.main.transform.position + (Vector3.down * 1.15f);
			transform.LookAt (point);
			canSeePlayer = CanSeeEntity (out p);
			if (canSeePlayer) {
				return;
			}
		}
	}

	private enum AIStrategy {
		GuardPoint,
		FollowPlayer
	}
}
