using UnityEngine;

public abstract class Weapon {
	public abstract float FireDelay {get;}
	public abstract float ReloadTime {get;}
	public abstract int MaxAmmo {get;}
	public abstract int ClipSize {get;}
	public abstract GameObject ModelPrefab {get;}
	public abstract void Fire ();
}
