using UnityEngine;
using System.Linq;

public class Projectile : MonoBehaviour {

    public float MaxDamage = 100f;      //Base Damage for direct hits
    public float DamageRange = 3f;      //Max Range for explosion
    public float ExplosiveLift = 1f;
    public bool IsGrenade = false;
    public float ExplosionTime = 2f;

    public void Start() {
        Destroy(gameObject, ExplosionTime);
    }


    public void OnCollisionEnter(Collision collision) {
        //Explosion Animation

        Vector3 explosionLocation = transform.position; //Location for Explosion

        //Get enemies within range of explosion
        Collider[] colliders = Physics.OverlapSphere(explosionLocation, DamageRange);

        //Check all colliders
        foreach (Collider c in colliders.Where(c => c.attachedRigidbody))
        {
            //Deal Damage

            //Squirrell enemyPlayer = c.GetComponent<Squirrell>();
            //enemyPlayer.damage(getProjectileDamage());

            //Add Explosion Force?
            //c.attachedRigidbody.AddExplosionForce(MaxDamage, explosionLocation, DamageRange, ExplosiveLift);

            //Destroy Projectile
            if (!IsGrenade)
                Destroy(gameObject);
        }
    }
}
