using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

[UsedImplicitly] public class Squirrell : MonoBehaviour {

	public const float SPEED = 2f;
	public const float JUMP_HEIGHT = 4f;
	public const float CLIMB_SPEED = 2f;
	Vector3 movement;

	Collider currentTree;
	HashSet<Collider> currentlyColliding;

	private Rigidbody playerRigidbody;
	private int floorMask;
    private Rigidbody rigidBody;
    public bool canJump = true;
    private NetworkView networkView;
	// new NetworkView networkView;
	//private float camRayLength = 100f;

	float h, v;

	private int maxHealth = 100;	//max health. Change as needed
	private int currHealth;			//current health
	private int maxAmmo;			//max ammo. Change as needed. How does this change with weapon?
	private int currAmmo;			//current ammo

	//getters and setters for health and ammo
	public int getHealth()
	{
		return(this.currHealth);
	}
	public int getAmmo()
	{
		return(this.currAmmo);
	}
	public void setHealth(int val)
	{
		this.currHealth = val;
	}
	public void setMaxAmmo(int val)
	{
		this.maxAmmo = val;
	}
	public void setAmmo(int val)
	{
		this.currAmmo = val;
	}

    /* Weapon Code Begins */
    float fireTimer;

    public Transform GunHand;
    private GameObject CurrentWeapon;

    float FireDelay;

    bool IsTraceFire;

    float TraceDamage;
    float TraceRange;

    string ProjectileName;

    void Equip(Weapon wep)
    {
        FireDelay = wep.getFireDelay();
        CurrentWeapon = wep.ModelPrefab;
    }

    void Shoot()
    {

    }
    /* Weapon Code Ends */
	[UsedImplicitly] void Start () {
        fireTimer = 0f;

		rigidBody = GetComponent<Rigidbody>();
		rigidBody.freezeRotation = true;
		networkView = GetComponent<NetworkView>();

		playerRigidbody = GetComponent<Rigidbody>();
		currentlyColliding = new HashSet<Collider>();
		// currentTree = null;

		if (networkView.isMine) {
			GameObject cameraObj = new GameObject("squirrell camera");
			cameraObj.transform.parent = transform;
			Camera camera = cameraObj.AddComponent<Camera>();
			camera.transform.localPosition = new Vector3(0, 1, -10);
			cameraObj.AddComponent<CameraVert>();
			
		}
	}

	[UsedImplicitly] void Update () {
		if (networkView.isMine && !Game.showMenu)
			InputMovement();

        fireTimer += Time.deltaTime;

        if (Input.GetButton("Fire1") && fireTimer >= FireDelay && Time.timeScale != 0)
        {
            Shoot();
        }

        /*if (timer >= FireDelay * effectsDisplayTime)
        {
            DisableEffects();
        }*/
	}
	
	void InputMovement () {

		h = Input.GetAxis("Horizontal");
		v = Input.GetAxis("Vertical");

		if (currentTree == null) {
			if (Input.GetKeyDown("space") && canJump)
            {
				Vector3 vel = playerRigidbody.velocity;
				vel.y = JUMP_HEIGHT;
				playerRigidbody.velocity = vel;
			}
			Move();
		}
		if (!Game.showMenu)
			Turning();
	}

	void OnCollisionEnter(Collision collision) {
		//string tag = collision.collider.gameObject.tag;
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
			    playerRigidbody.velocity.Set(playerRigidbody.velocity.x, playerRigidbody.velocity.y, CLIMB_SPEED);
			}
		}

        // Only allow jumping on the ground and trees
        if (collision.transform.tag == "Climbable")
	    {
            canJump = true;
	    }
	}

	bool WantsClimbing() { return Input.GetKey(KeyCode.LeftShift) && v > 0.5; }

	void OnCollisionExit(Collision collision) {
		currentlyColliding.Remove(collision.collider);
		if (collision.collider == currentTree) {
			currentTree = null;
			playerRigidbody.useGravity = true;
		}
        canJump = false;
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
