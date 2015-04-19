using UnityEngine;
using System.Collections;

public abstract class TraceWeapon : Weapon
{
    Ray shootRay;
    RaycastHit shootHit;
    //int shootableMask;

    public abstract int getTraceDamage();
    public abstract float getTraceRange();

    public abstract string getProjectileName();

    public override void Fire()
    {
        shootRay.origin = Game.squirrell.transform.position;
        shootRay.direction = Game.squirrell.transform.rotation.eulerAngles;
        //gunAudio.Play();

        //gunLight.enabled = true;

        //gunParticles.Stop();
        //gunParticles.Play();

        //shootRay.origin = GetComponent<Transform>().transform;
        //shootRay.direction = GetComponent<Transform>.forward;

        //gunLine.enabled = true;
        //gunLine.SetPosition(GetComponent(MuzzleTag).transform, GetComponent(MuzzleTag).position);


        if (Physics.Raycast(shootRay, out shootHit, getTraceRange(), LayerMask.GetMask("Everything")))
        {
            Squirrell enemyPlayer = shootHit.collider.GetComponent<Squirrell>();
            
            enemyPlayer.damage(getTraceDamage());

            //gunLine.SetPosition(1, shootHit.point);
        }
        else
        {
            //gunLine.SetPosition(1, shootRay.origin + shootRay.direction * TraceRange);
        }
    }
}
