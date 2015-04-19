using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

[UsedImplicitly] public class Squirrell : MonoBehaviour {

	public const float SPEED = 6f;
	public const float JUMP_HEIGHT = 10f;
	public const float CLIMB_SPEED = 2f;
	Vector3 movement;

	Collider currentTree;
	HashSet<Collider> currentlyColliding;

	private Rigidbody playerRigidbody;
	private int floorMask;
	//private float camRayLength = 100f;

	float h, v;

	[UsedImplicitly] void Start () {
		rigidBody = GetComponent<Rigidbody>();
		rigidBody.freezeRotation = true;
		networkView = GetComponent<NetworkView>();

		playerRigidbody = GetComponent<Rigidbody>();
		currentlyColliding = new HashSet<Collider>();
		currentTree = null;

		if (networkView.isMine) {
			GameObject cameraObj = new GameObject("squirrell camera");
			Camera camera = cameraObj.AddComponent<Camera>();
			camera.transform.localPosition = new Vector3(0, 1, -2);
			cameraObj.AddComponent<CameraVert>();
			cameraObj.transform.parent = this.transform;
		}
	}
	[UsedImplicitly] void Update () {
		if (networkView.isMine)
			InputMovement();
	}
	Rigidbody rigidBody;
	new NetworkView networkView;
	void InputMovement () {

		h = Input.GetAxis("Horizontal");
		v = Input.GetAxis("Vertical");

		if (currentTree == null) {
			if (Input.GetKeyDown("space") && currentlyColliding.Count > 0) {
				Vector3 vel = playerRigidbody.velocity;
				vel.y = JUMP_HEIGHT;
				playerRigidbody.velocity = vel;
			}
			Move();
		}

		Turning();

	}

	void OnCollisionEnter(Collision collision) {
		string tag = collision.collider.gameObject.tag;
		currentlyColliding.Add(collision.collider);
	}

	void OnCollisionStay(Collision collision) {
		if (collision.collider == currentTree) {
			if (!WantsClimbing()) {
				currentTree = null;
				playerRigidbody.useGravity = true;
				return;
			}
		} else {
			if (collision.collider.gameObject.tag == "Tree" && WantsClimbing()) {
				currentTree = collision.collider;
				playerRigidbody.useGravity = false;
				Vector3 vel = playerRigidbody.velocity;
				vel.y = CLIMB_SPEED;
				playerRigidbody.velocity = vel;
			}
		}
	}

	bool WantsClimbing() { return Input.GetKey(KeyCode.LeftShift) && v > 0.5; }

	void OnCollisionExit(Collision collision) {
		currentlyColliding.Remove(collision.collider);
		if (collision.collider == currentTree) {
			currentTree = null;
			playerRigidbody.useGravity = true;
		}
	}

	void Move() {
		movement.Set(h, 0f, v);
		//movement = movement.normalized;
		if (Input.GetKey(KeyCode.LeftShift)) {
			movement.z = movement.z * 2;
		}
		movement = playerRigidbody.rotation * movement;
		movement *= SPEED * Time.deltaTime;

		playerRigidbody.MovePosition(transform.position + movement);
	}

	void Turning() {
		float rotX = Input.GetAxis("Mouse X");
		playerRigidbody.MoveRotation(playerRigidbody.rotation * Quaternion.AngleAxis(rotX * 2, Vector3.up));
	}
}
