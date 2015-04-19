using UnityEngine;

public abstract class TraceWeapon : Weapon {
	public abstract int TraceDamage {get;}
	public abstract float TraceRange {get;}
	public override void Fire () {
		MonoBehaviour.print("bang");
		var shootRay = new Ray {
			origin = Game.assets.squirrell.transform.eulerAngles,
			direction = Game.assets.squirrell.transform.forward
		};
		// ReSharper disable once RedundantAssignment
		var shootHit = new RaycastHit();
		if (Physics.Raycast(shootRay, out shootHit, TraceRange, LayerMask.GetMask("Everything"))) {
			MonoBehaviour.print("hit: " + shootHit);
			var enemyPlayer = shootHit.collider.GetComponent<Squirrell>();
			if (enemyPlayer != null)
				enemyPlayer.Damage(TraceDamage);
		}
	}
}
