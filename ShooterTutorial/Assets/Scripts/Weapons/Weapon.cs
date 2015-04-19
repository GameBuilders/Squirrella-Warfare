using UnityEngine;
using System.Collections;

public abstract class Weapon
{
    public abstract float getFireDelay();

    public abstract bool getIsTraceFire();

    public abstract float getTraceDamage();
    public abstract float getTraceRange();

    public abstract string getProjectileName();

    public abstract Transform getWeaponMesh();

    //[UsedImplicitly]
   // public Transform muzzle;

    /*GameObject Projectile;

    float timer;
    float effectsDisplayTime = 0.2f;

    Ray shootRay;
    RaycastHit shootHit;
    int shootableMask;

    //Cache components
    ParticleSystem gunParticles;
    LineRenderer gunLine;
    AudioSource gunAudio;
    Light gunLight;

    public WeaponInfo weaponInfo;
    void Awake()
    {
        if (!IsTraceFire) //Setup the projectile if necessary
            Projectile = GameObject.Find(ProjectileName);

        shootableMask = LayerMask.GetMask("Shootable");
        gunParticles = GetComponent<ParticleSystem>();
        gunLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponent<Light>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (Input.GetButton("Fire1") && timer >= FireDelay && Time.timeScale != 0)
        {
            Shoot();
        }

        if (timer >= FireDelay * effectsDisplayTime)
        {
            DisableEffects();
        }
    }

    void Shoot()
    {
        timer = 0f;

        gunAudio.Play();

        gunLight.enabled = true;

        gunParticles.Stop();
        gunParticles.Play();

        if (IsTraceFire)
            ShootTrace();
        else
            ShootProjectile();
    }

    void ShootTrace()
    {
        //shootRay.origin = GetComponent<Transform>().transform;
        //shootRay.direction = GetComponent<Transform>.forward;

        gunLine.enabled = true;
        //gunLine.SetPosition(GetComponent(MuzzleTag).transform, GetComponent(MuzzleTag).position);


        if (Physics.Raycast(shootRay, out shootHit, TraceRange, shootableMask))
        {
            EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // enemyHealth.TakeDamage(damagePerShot, shootHit.point);
            }
            gunLine.SetPosition(1, shootHit.point);
        }
        else
        {
            gunLine.SetPosition(1, shootRay.origin + shootRay.direction * TraceRange);
        }
    }

    void ShootProjectile()
    {

    }

    public void DisableEffects()
    {
        gunLine.enabled = false;
        gunLight.enabled = false;
    }*/
}
