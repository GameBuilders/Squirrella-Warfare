using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public Transform GunHand;
    private Weapon CurrentWeapon;

    float FireDelay;

    bool IsTraceFire;

    float TraceDamage;
    float TraceRange;

    string ProjectileName;

    void Equip(Weapon wep)
    {
        FireDelay = wep.getFireDelay();
        IsTraceFire = wep.getIsTraceFire();
        TraceDamage = wep.getTraceDamage();
        TraceRange = wep.getTraceRange();
        ProjectileName = wep.getProjectileName();
    }
}
