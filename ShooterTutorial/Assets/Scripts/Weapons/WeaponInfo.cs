using UnityEngine;
using System.Collections;

public class WeaponInfo : MonoBehaviour
{

	public float FireDelay;

    public bool IsTraceFire;
    public float TraceDamage;

    public string ProjectileName;

    GameObject Projectile;
    void Start()
    {
        //Setup the projectile component if it is used
        if(!IsTraceFire)
            Projectile = GameObject.Find(ProjectileName);
    }
}
