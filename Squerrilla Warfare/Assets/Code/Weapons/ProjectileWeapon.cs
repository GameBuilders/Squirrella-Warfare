using UnityEngine;

public abstract class ProjectileWeapon : Weapon {
	public abstract Transform ProjectilePrefab {get;}
	public abstract float Velocity {get;}
	public override void Fire () {
		var instantiated = (Transform) MonoBehaviour.Instantiate(ProjectilePrefab, Game.assets.squirrell.transform.position, Game.assets.squirrell.transform.rotation);
		var rigidbody = instantiated.GetComponent<Rigidbody>();
		rigidbody.velocity = owner.transform.TransformDirection(Vector3.forward * Velocity);
	}
}
