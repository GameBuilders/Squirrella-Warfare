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
    private int maxClip;
    private int currClip;

    GameObject weaponPrefab = null;

	//getters and setters for health and ammo
	public int getHealth()
	{
		return currHealth;
	}
	public int getAmmo()
	{
		return currAmmo;
	}
    public int getMaxAmmo()
    {
        return maxAmmo;
    }
    public int getMaxClip()
    {
        return maxClip;
    }
    public int getClip()
    {
        return currClip;
    }
	public void setHealth(int val)
	{
		currHealth = val;
	}
    public void damage(int val)
    {
        currHealth -= val;
    }
	public void setMaxAmmo(int val)
	{
		maxAmmo = val;
	}
	public void setAmmo(int val)
	{
		currAmmo = val;
	}
    public void setMaxClip(int val)
    {
        maxClip = val;
    }
    public void setClip(int val)
    {
        currClip = val;
    }
    public void useAmmo()
    {
        currClip -= 1;
    }

    /* Weapon Code Begins */
    float fireTimer;

    public Transform GunHand;
    private Weapon CurrentWeapon;

    float FireDelay;

    bool IsTraceFire;

    float TraceDamage;
    float TraceRange;

    string ProjectileName;

    void Equip(Weapon wep)
    {
        FireDelay = wep.getFireDelay();
        CurrentWeapon = wep;
        setMaxAmmo(wep.getMaxAmmo());
        setMaxClip(wep.getClipSize());

        if (weaponPrefab != null) //Destroy our previous prefab if it exists
            Network.Destroy(weaponPrefab);

        weaponPrefab = Extensions.InstantiateChild(this.gameObject, wep.getModelPrefab()) as GameObject;

        weaponPrefab = wep.getModelPrefab();
    }

    void Shoot()
    {
        fireTimer = 0f;
        useAmmo();
        Debug.Log(getClip());
        CurrentWeapon.Fire();
    }

    void Reload()
    {
        if (getAmmo() >= getMaxClip() - getClip()) //Player can fully reload
        {
            setAmmo(getAmmo() - (getMaxClip() - getClip()));
            setClip(getMaxClip());

            fireTimer = -CurrentWeapon.getReloadTime() + CurrentWeapon.getFireDelay();
        }
        else if(getAmmo() > 0) //Player can partially reload
        {
            setClip(getClip() + getAmmo());
            setAmmo(0);

            fireTimer = -CurrentWeapon.getReloadTime() + CurrentWeapon.getFireDelay();
        }
        //Player can't reload? Sucks to suck.
    }
    /* Weapon Code Ends */

	[UsedImplicitly] void Start () {
        Equip(new AssaultRifle());
        if (maxAmmo == 0) //Prevents warning. Remove when implemented!
        {
            fireTimer = 0; 
        }

        setAmmo(getMaxAmmo());
        setClip(getMaxClip());
        fireTimer = 0f;
        currHealth = maxHealth;

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

        if (Input.GetButton("Fire1") && fireTimer >= FireDelay && Time.timeScale != 0 && getClip() > 0)
        {
            Shoot();
        }
        else if (Input.GetButton("Reload") && getClip() < getMaxClip())
            Reload();

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
