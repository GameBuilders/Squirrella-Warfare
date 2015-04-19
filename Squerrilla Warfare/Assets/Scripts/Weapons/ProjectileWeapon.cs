using UnityEngine;
using System.Collections;

public abstract class ProjectileWeapon : Weapon
{
    public override void Fire()
    {

    }

    public abstract string getProjectileName();
	
}
