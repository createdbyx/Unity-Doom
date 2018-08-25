using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ThingController))]
public class EntityAnimator : MonoBehaviour {

	private const float animationDistanceCull = 150;
	private const float animationUpdateRate = 0.1f;


	[SerializeField] private AnimationSet[] animationSets = null;

	private Dictionary<string, AnimationSet> animationSetDictionary = new Dictionary<string, AnimationSet>();

	private ThingController thingController;
	private string currentAnimationSet;
	private int animationIdx = 0;
	[HideInInspector]
	public bool StopAnimationUpdateAfterCurrent = false;

	void Awake ()
	{
		for (int i = 0; i < animationSets.Length; i++) {
			animationSets[i].Init();
			animationSetDictionary.Add(animationSets[i].animationId, animationSets[i]);
		}
		SetAnimationSet("IDLE");

		thingController = GetComponent<ThingController>();
		StartCoroutine(DoAnimationFrame());
	}

	public void SetAnimationSet (string setId)
	{
		if (setId == currentAnimationSet) return;
		if (animationSetDictionary.ContainsKey (setId)) {
			currentAnimationSet = setId;
			animationIdx = 0;
			return;
		}
		Debug.LogError("Invalid Animation Set: " + setId + " (" + name + ")");
	}

	// Update Animation Frame continuially
	private IEnumerator DoAnimationFrame ()
	{
		if (Camera.main != null) {
			if (Vector3.Distance (Camera.main.transform.position, transform.position) < animationDistanceCull) {
				thingController.SetTexture (GetDirectionAnimationTexture ());
				animationIdx++;
			}
		}

		yield return new WaitForSeconds(animationUpdateRate);
		StartCoroutine(DoAnimationFrame());
	}

	private Texture GetDirectionAnimationTexture ()
	{
		
		int billboardIdx = AnimIdxBillboard ();
		int rotationIdx = AnimIdxRotation ();

		int animIdx = billboardIdx + rotationIdx;
		if (animIdx > 7) {
			animIdx -= 8;
		}

		// Find animation and check we aren't outside on the idx bounds
		Texture[] currentAnimation = AnimationFromRotationIdx (animIdx, animationSetDictionary [currentAnimationSet]);
		if (animationIdx >= currentAnimation.Length) {
			if (StopAnimationUpdateAfterCurrent) {
				animationIdx = currentAnimation.Length - 1;
				StopAllCoroutines();
			} else {
				animationIdx = 0;
			}
		}

		UnityEvent outEvent; // see if there's an event to call on this frame
		if (animationSetDictionary [currentAnimationSet].GetEventAtIdx (animationIdx, out outEvent)) {
			outEvent.Invoke();
		}

		return currentAnimation[animationIdx];
	}

	private Texture[] AnimationFromRotationIdx (int idx, AnimationSet animSet)
	{
		switch (idx) {
			case 0:
			return animSet.forwardSprites;
			case 1:
			thingController.SetSpriteDirection (-1);
			return animSet.forwardRightSprites;
			case 2:
			thingController.SetSpriteDirection (-1);
			return animSet.rightSprites;
			case 3:
			thingController.SetSpriteDirection (-1);
			return animSet.backRightSprites;
			case 4:
			return animSet.backSprites; 
			case 5:
			thingController.SetSpriteDirection (1);
			return animSet.backRightSprites;
			case 6:
			thingController.SetSpriteDirection (1);
			return animSet.rightSprites;
			case 7:
			thingController.SetSpriteDirection (1);
			return animSet.forwardRightSprites;
		}

		Debug.LogError("Invalid Idx: " + idx);
		return animSet.forwardSprites;
	}

	private int AnimIdxRotation () {
		Vector3 rotation = transform.eulerAngles;
		if (rotation.y >= 360) {
			rotation.y -= 360;
		}
		if (rotation.y < 0) {
			rotation.y += 360;
		}
		return 8 - (Mathf.RoundToInt((rotation.y/360f) * 8f));
	}

	private int AnimIdxBillboard ()
	{
		// Heading (Camera.current makes the billboard effect work on scene view)
		Vector3 heading = Vector3.zero;
		if (Camera.current != null) heading = Camera.current.transform.position - transform.position;
		if (Camera.main != null) heading = Camera.main.transform.position - transform.position;

		float distance = heading.magnitude;
		Vector3 direction = heading / distance;

		float xAbs = Mathf.Abs (direction.x);
		float zAbs = Mathf.Abs (direction.z);

		// Find Animation set based on camera heading
		int animIdx = 0;
		if (xAbs > 0.75f) {
			if (zAbs > 0.5f) {
				if (direction.z > 0) {
					if (direction.x > 0) {
						animIdx = 1;
					} else {
						animIdx = 7;
					}
				} else {
					if (direction.x > 0) {
						animIdx = 3;
					} else {
						animIdx = 5;
					}
				}
			} else {
				if (direction.x > 0) {
					animIdx = 2;
				} else {
					animIdx = 6;
				}
			}
		} else {
			if (direction.z > 0) {
				animIdx = 0;
			} else {
				animIdx = 4;
			}
		}
		return animIdx;
	}

	[System.Serializable]
	private struct AnimationSet {
		public string animationId;
		public AnimationEvent[] events;
		[HeaderAttribute("Sprite IDs")]
		public string[] forward;
		public string[] forwardRight;
		public string[] right;
		public string[] backRight;
		public string[] back;


		[HideInInspector]
		public Texture[] forwardSprites;
		[HideInInspector]
		public Texture[] forwardRightSprites;
		[HideInInspector]
		public Texture[] rightSprites;
		[HideInInspector]
		public Texture[] backRightSprites;
		[HideInInspector]
		public Texture[] backSprites;

		private Dictionary<int, UnityEvent> eventDictionary;

		public bool GetEventAtIdx (int idx, out UnityEvent outEvent) {
			if (eventDictionary.TryGetValue(idx, out outEvent)) {
				return true;
			}
			outEvent = null;
			return false;
		}


		public void Init () // I figure it's faster than calling the TextureLoader every update
		{
			// Sprites
			forwardSprites = new Texture[forward.Length];
			for (int i = 0; i < forward.Length; i++) {
				forwardSprites [i] = TextureLoader.Instance.GetSpriteTexture (forward [i]);
			}
			forwardRightSprites = new Texture[forwardRight.Length];
			for (int i = 0; i < forwardRight.Length; i++) {
				forwardRightSprites [i] = TextureLoader.Instance.GetSpriteTexture (forwardRight [i]);
			}
			rightSprites = new Texture[right.Length];
			for (int i = 0; i < right.Length; i++) {
				rightSprites [i] = TextureLoader.Instance.GetSpriteTexture (right [i]);
			}
			backRightSprites = new Texture[backRight.Length];
			for (int i = 0; i < backRight.Length; i++) {
				backRightSprites [i] = TextureLoader.Instance.GetSpriteTexture (backRight [i]);
			}
			backSprites = new Texture[back.Length];
			for (int i = 0; i < back.Length; i++) {
				backSprites [i] = TextureLoader.Instance.GetSpriteTexture (back [i]);
			}

			//Events
			eventDictionary = new Dictionary<int, UnityEvent>();
			foreach (AnimationEvent e in events) {
				eventDictionary.Add(e.frame, e.onFrame);
			}
		}
	}

	[System.Serializable]
	private struct AnimationEvent {
		public int frame;
		public UnityEvent onFrame;
	}
}