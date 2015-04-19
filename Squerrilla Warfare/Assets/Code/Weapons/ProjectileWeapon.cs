using UnityEngine;
using System.Collections;

public abstract class ProjectileWeapon : Weapon
{
    Rigidbody Projectile;
    Rigidbody projectile;

    public abstract string ProjectileName;

    public abstract int ProjectileDamage;

    public abstract float ProjectileRange;

    public abstract float DetonationTime;

    public abstract float Velocity;


    public override void Fire()
    {
        projectile = (Rigidbody) Instantiate(Projectile, Game.assets.squirrell.transform.position, Game.assets.squirrell.transform.rotation);
        projectile.velocity = transform.TransformDirection(Vector3.forward * Velocity);

    }

    
	
}
