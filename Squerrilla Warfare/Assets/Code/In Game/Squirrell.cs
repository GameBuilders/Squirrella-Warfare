using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[UsedImplicitly] public class Squirrell : MonoBehaviour {
	public const float speed = 2f;
	public const float jumpHeight = 4f;
	public const float climbSpeed = 2f;
	public bool canJump = true;
	int totalAmmo;
	int ammoInClip;
	HashSet<Collider> currentlyColliding;
	/* Weapon Code Ends */
	Collider currentTree;
	int currentHealth;
	int CurrentHealth {
		get {return currentHealth;}
		set {
			currentHealth = value;
			if (currentHealth <= 0)
				Die();
		}
	}
	float FireDelay {get {return currentWeapon.FireDelay;}}
	/* Weapon Code Begins */
	float fireTimer;
	public Transform gunHand;
	// new NetworkView networkView;
	//private float camRayLength = 100f;
	float h, v;
	int MaxAmmo {get {return currentWeapon.MaxAmmo;}}
	int MaxClip {get {return currentWeapon.ClipSize;}}
	const int maxHealth = 100; //max health. Change as needed
	Vector3 movement;
	new NetworkView networkView;
	Rigidbody playerRigidbody;
	Rigidbody rigidBody;
	GameObject weaponPrefab = null;
	//getters and setters for health and ammo
	public int Ammo {get {return totalAmmo;}}
	public void Damage (int amount) {CurrentHealth -= amount;}
	public void Damage (float amount) {Damage(Mathf.RoundToInt(amount));}
	public void UseAmmo () {ammoInClip -= 1;}
	Weapon currentWeapon;
	void Die () {}//todo
	public Weapon CurrentWeapon {get {return currentWeapon;}
		set {
			currentWeapon = value;
			if (weaponPrefab != null) //Destroy our previous prefab if it exists
				Network.Destroy(weaponPrefab);
			weaponPrefab = gameObject.InstantiateChild(value.ModelPrefab);
			weaponPrefab = value.ModelPrefab;
		}
	}
	void Shoot () {
		fireTimer = 0f;
		UseAmmo();
		Debug.Log(CurrentHealth);
		currentWeapon.Fire();
	}
	bool CanFullyReload {get {return Ammo >= MaxClip - ammoInClip;}}
	void TryToReload () {
		if (CanFullyReload) {
			totalAmmo = Ammo - MaxClip - ammoInClip;
			ammoInClip =(MaxClip);
			fireTimer = -currentWeapon.ReloadTime + currentWeapon.FireDelay;
		}
		else if (Ammo > 0) {
			ammoInClip =(ammoInClip + Ammo);
			totalAmmo = 0;
			fireTimer = currentWeapon.FireDelay - currentWeapon.ReloadTime;
		}
	}
	[UsedImplicitly] void Start () {
		CurrentWeapon = new AssaultRifle();
		if (MaxAmmo == 0)//Prevents warning. Remove when implemented!
			fireTimer = 0;
		totalAmmo = MaxAmmo;
		ammoInClip = MaxClip;
		fireTimer = 0f;
		CurrentHealth = maxHealth;
		rigidBody = GetComponent<Rigidbody>();
		rigidBody.freezeRotation = true;
		networkView = GetComponent<NetworkView>();
		playerRigidbody = GetComponent<Rigidbody>();
		currentlyColliding = new HashSet<Collider>();
		// currentTree = null;
		if (networkView.isMine) {
			var cameraObj = new GameObject("squirrell camera");
			cameraObj.transform.parent = transform;
			var camera = cameraObj.AddComponent<Camera>();
			camera.transform.localPosition = new Vector3(0, 1, -10);
			cameraObj.AddComponent<CameraVert>();
		}
	}
	[UsedImplicitly] void Update () {
		if (networkView.isMine && !Game.showMenu)
			InputMovement();
		fireTimer += Time.deltaTime;
		if (Input.GetButton("Fire1"))
			TryToShoot();
		else if (Input.GetButton("Reload") && ammoInClip < MaxClip)
			TryToReload();
	}
	void TryToShoot () {
		if (ammoInClip == 0)
			TryToReload();
		else if (fireTimer >= FireDelay)
			Shoot();
	}
	void InputMovement () {
		h = Input.GetAxis("Horizontal");
		v = Input.GetAxis("Vertical");
		if (currentTree == null) {
			if (Input.GetKeyDown("space") && canJump) {
				var vel = playerRigidbody.velocity;
				vel.y = jumpHeight;
				playerRigidbody.velocity = vel;
			}
			Move();
		}
		if (!Game.showMenu)
			Turning();
	}
	[UsedImplicitly] void OnCollisionEnter (Collision collision) {
		//string tag = collision.collider.gameObject.tag;
		currentlyColliding.Add(collision.collider);
	}
	[UsedImplicitly] void OnCollisionStay (Collision collision) {
		if (collision.collider == currentTree) {
			if (!WantsClimbing()) {
				currentTree = null;
				playerRigidbody.useGravity = true;
				return;
			}
		}
		else {
			if (collision.collider.gameObject.tag == "Tree" && WantsClimbing()) {
				currentTree = collision.collider;
				playerRigidbody.useGravity = false;
				playerRigidbody.velocity.Set(playerRigidbody.velocity.x, playerRigidbody.velocity.y, climbSpeed);
			}
		}
		// Only allow jumping on the ground and trees
		if (collision.transform.tag == "Climbable")
			canJump = true;
	}
	bool WantsClimbing () {return Input.GetKey(KeyCode.LeftShift) && v > 0.5;}
	[UsedImplicitly] void OnCollisionExit (Collision collision) {
		currentlyColliding.Remove(collision.collider);
		if (collision.collider == currentTree) {
			currentTree = null;
			playerRigidbody.useGravity = true;
		}
		canJump = false;
	}
	void Move () {
		movement.Set(h, 0f, v);
		//movement = movement.normalized;
		if (Input.GetKey(KeyCode.LeftShift)) {
			movement.z = movement.z * 2;
		}
		movement = playerRigidbody.rotation * movement;
		movement *= speed * Time.deltaTime;
		playerRigidbody.MovePosition(transform.position + movement);
	}
	void Turning () {
		var rotX = Input.GetAxis("Mouse X");
		playerRigidbody.MoveRotation(playerRigidbody.rotation * Quaternion.AngleAxis(rotX * 2, Vector3.up));
	}
}
