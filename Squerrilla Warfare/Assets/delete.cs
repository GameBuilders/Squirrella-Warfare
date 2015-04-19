using UnityEngine;
using System.Collections;

public class delete : MonoBehaviour
{

    private AudioSource clip;
    
	// Use this for initialization
	void Start ()
	{
	    clip = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (!clip.isPlaying)
	    {
	        Destroy(gameObject);
	    }
	}
}
