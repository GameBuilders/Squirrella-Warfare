using UnityEngine;
using System.Collections;

public class AssaultRifle : TraceWeapon
{
    public AssaultRifle() {}
    public override float getFireDelay() {return 0.3f;}
    public override float getReloadTime() {return 3.0f;} 

    public override int getTraceDamage() { return 10; }
    public override float getTraceRange() { return 1000.0f; }

    public override int getMaxAmmo() { return 75; }
    public override int getClipSize() { return 25; }

    public override string getProjectileName() { return ""; }
    public override GameObject getModelPrefab() {  return Game.assets.arPrefab; }
   // public override Transform getWeaponMesh() { return Transform(); }
}
