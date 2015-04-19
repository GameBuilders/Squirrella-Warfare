using UnityEngine;

public class AssaultRifle : TraceWeapon {
	public override GameObject ModelPrefab {get {return Game.assets.arPrefab;}}
	public override float FireDelay {get {return 0.3f;}}
	public override float ReloadTime {get {return 3.0f;}}
	public override int TraceDamage {get {return 10;}}
	public override float TraceRange {get {return 1000.0f;}}
	public override int MaxAmmo {get {return 75;}}
	public override int ClipSize {get {return 25;}}

    public override string Description { get { return "Developed by the Acorn Union, the Acorn-47 has been the go to squirrel-annihilating weapon of choice; powerful and reliable"; } }

    public override string WeaponName { get { return "Acorn-47"; } }
}
