using UnityEngine;
using JetBrains.Annotations;

[UsedImplicitly] public class Squirrell : MonoBehaviour {
	const float speed = 4f;
	[UsedImplicitly] void Start () {
		rigidBody = GetComponent<Rigidbody>();
		rigidBody.freezeRotation = true;
		networkView = GetComponent<NetworkView>();
	}
	[UsedImplicitly] void Update () {
		if (networkView.isMine)
			InputMovement();
	}
	Rigidbody rigidBody;
	new NetworkView networkView;
	void InputMovement () {
		var distance = speed * Time.deltaTime;
		if (Input.GetKey(KeyCode.W))
			rigidBody.MovePosition(rigidBody.position + Vector3.forward * distance);
		if (Input.GetKey(KeyCode.S))
			rigidBody.MovePosition(rigidBody.position - Vector3.forward * distance);
		if (Input.GetKey(KeyCode.D))
			rigidBody.MovePosition(rigidBody.position + Vector3.right * distance);
		if (Input.GetKey(KeyCode.A))
			rigidBody.MovePosition(rigidBody.position - Vector3.right * distance);
	}
}
