using UnityEngine;
using System.Collections;

public class ProjectileMovement : MonoBehaviour
{
    public float linearSpeed;

	void Start ()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * linearSpeed * Time.deltaTime;
	}
}
