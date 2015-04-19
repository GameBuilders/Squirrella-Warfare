using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[UsedImplicitly] public class Squirrell : MonoBehaviour {
	const float speed = 2f;
	const float jumpHeight = 4f;
	const float climbSpeed = 2f;
	bool canJump = true;
	HashSet<Collider> currentlyColliding;
	/* Weapon Code Ends */
	Collider currentTree;
	float currentHealth;
	float CurrentHealth {
		get {return currentHealth;}
		set {
			currentHealth = value;
			if (currentHealth <= 0)
				Die();
		}
	}
	// ReSharper disable MemberCanBePrivate.Global
	public Weapon MainWeapon {
		get {return mainWeapon;}
		set {
			OnRelinquishWeapon(mainWeapon);
			mainWeapon = value;
			OnClaimWeapon(mainWeapon);
		}
	}
	Weapon mainWeapon;
	public Weapon SecondaryWeapon {
		get {return secondaryWeapon;}
		set {
			OnRelinquishWeapon(secondaryWeapon);
			secondaryWeapon = value;
			OnClaimWeapon(secondaryWeapon);
		}
	}
	Weapon secondaryWeapon;
	void OnRelinquishWeapon (Weapon toRelinquish) {
		if (toRelinquish != null)
			toRelinquish.owner = null;
	}
	void OnClaimWeapon (Weapon toClaim) {
		if (toClaim != null)
			toClaim.owner = this;
	}
	// ReSharper restore MemberCanBePrivate.Global
	int MaxAmmo {get {return currentWeapon.MaxAmmo;}}
    int MaxClip {get {return currentWeapon.ClipSize;}}

	float FireDelay {get {return currentWeapon.FireDelay;}}
	/* Weapon Code Begins */
	float fireTimer;
	[UsedImplicitly] public GameObject gunHand;
	// new NetworkView networkView;
	//private float camRayLength = 100f;
	float h, v;
	const int maxHealth = 100; //max health. Change as needed
	Vector3 movement;
	public new NetworkView networkView;
	Rigidbody playerRigidbody;
	Rigidbody rigidBody;
	GameObject weaponModel = null;
	//getters and setters for health and ammo
	[RPC] public void Damage (float amount) {
		print("I have been damaged.");
		CurrentHealth -= amount;
	}
	int Ammo {
        get {return CurrentWeapon.totalAmmo;}
        set {CurrentWeapon.totalAmmo = value;}
    }
	int AmmoInClip { 
        get {return CurrentWeapon.ammoInClip;}
        set {CurrentWeapon.ammoInClip = value;}
    }
	void UseAmmo () {CurrentWeapon.ammoInClip -= 1;}
	Weapon currentWeapon;
	void Die () {
		MonoBehaviour.print("What a world.");
		Game.OnDied();
	}//todo
	// ReSharper disable once MemberCanBePrivate.Global
	public Weapon CurrentWeapon {get {return currentWeapon;}
		set {
			currentWeapon = value;
			if (weaponModel != null) //Destroy our previous prefab if it exists
				Network.Destroy(weaponModel);
			weaponModel = gunHand.InstantiateChild(value.ModelPrefab);
			weaponModel = value.ModelPrefab;
		}
	}
	void Shoot () {
		fireTimer = 0f;
		UseAmmo();
		Debug.Log(CurrentHealth);
		currentWeapon.Fire();
	}
	bool CanFullyReload {get {return Ammo >= MaxClip - AmmoInClip;}}
	void TryToReload () {
		if (CanFullyReload) {
			Ammo = Ammo - MaxClip - AmmoInClip;
			AmmoInClip =(MaxClip);
			fireTimer = -currentWeapon.ReloadTime + currentWeapon.FireDelay;
		}
		else if (Ammo > 0) {
			AmmoInClip =(AmmoInClip + Ammo);
			Ammo = 0;
			fireTimer = currentWeapon.FireDelay - currentWeapon.ReloadTime;
		}
	}//
	[UsedImplicitly] void Start () {
		MainWeapon = new AssaultRifle();
		SecondaryWeapon = new AssaultRifle();
		CurrentWeapon = MainWeapon;
		if (MaxAmmo == 0)//Prevents warning. Remove when implemented!
			fireTimer = 0;
		Ammo = MaxAmmo;
		AmmoInClip = MaxClip;
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
        else if (Input.GetButton("Reload") && AmmoInClip < MaxClip)
            TryToReload();
        else if (Input.GetButton("Slot1") && CurrentWeapon != MainWeapon)
            CurrentWeapon = MainWeapon;
        else if (Input.GetButton("Slot2") && CurrentWeapon != SecondaryWeapon)
            CurrentWeapon = SecondaryWeapon;
	}
	void TryToShoot () {
		if (AmmoInClip == 0)
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
