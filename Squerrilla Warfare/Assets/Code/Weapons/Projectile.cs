using System.Linq;
using UnityEngine;

public abstract class Projectile : MonoBehaviour {
	public abstract float DamageRange {get;}
	public abstract float ExplosionTime {get;}
	public abstract float ExplosiveLift {get;}
	public virtual bool IsGrenade {get {return false;}}
	public abstract float MaxDamage {get;}
	public void Start () {Destroy(gameObject, ExplosionTime);}
	public void OnCollisionEnter (Collision collision) {
		var explosionLocation = transform.position;
		var colliders = Physics.OverlapSphere(explosionLocation, DamageRange);
		foreach (var c in colliders.Where(c => c.attachedRigidbody)) {
			var enemyPlayer = c.GetComponent<Squirrell>();
			if (enemyPlayer != null)
				enemyPlayer.Damage(MaxDamage);
			//Add Explosion Force?
			//c.attachedRigidbody.AddExplosionForce(MaxDamage, explosionLocation, DamageRange, ExplosiveLift);
			if (!IsGrenade)
				Destroy(gameObject);
		}
	}
}
