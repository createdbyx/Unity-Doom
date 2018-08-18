using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(EntityAnimator))]
public class EntityAI : MonoBehaviour {

	private const float AIUpdateRate = 0.1f;
	private const float targetDistance = 150;

	[SerializeField] private float moveSpeed = 5;
	[SerializeField] private float attackDistance = 5;
	[SerializeField] private int health = 10;
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

	public void DoAttack() {
		
	}

	public void StopMovement() {
		movementOverride = true;
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

				if (Vector3.Distance (transform.position, targetPos) < targetDistance && Vector3.Distance (transform.position, targetPos) > attackDistance) {
					transform.LookAt (targetPos);
					animator.SetAnimationSet ("MOVE");
					isMoving = true;
				} else {
					animator.SetAnimationSet ("ATTACK");
					isMoving = false;
				}
			}

			StartCoroutine (UpdateAI ());
		}
	}
}
