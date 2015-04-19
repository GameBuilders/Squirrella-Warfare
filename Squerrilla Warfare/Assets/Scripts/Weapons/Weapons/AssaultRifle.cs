using UnityEngine;
using System.Collections;

public class AssaultRife : TraceWeapon
{
    public override float getFireDelay() {return 0.3f;}

    public override float getTraceDamage() { return 20.0f; }
    public override float getTraceRange() { return 1000.0f; }

    public override string getProjectileName() { return ""; }
    public override GameObject ModelPrefab {get {return Game.assets.arPrefab;}}
   // public override Transform getWeaponMesh() { return Transform(); }
}
