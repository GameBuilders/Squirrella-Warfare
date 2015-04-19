using UnityEngine;
using System.Collections;

public class CameraVert : MonoBehaviour {
	new Camera camera;
	// Use this for initialization
	void Start () {
		camera = GetComponent<Camera>();
	}
	// Update is called once per frame
	void Update () {
		if (!Game.showMenu) {
			float rotY = Input.GetAxis("Mouse Y");
			camera.transform.rotation = camera.transform.rotation * Quaternion.AngleAxis(rotY * 3, Vector3.left);
			Vector3 pos = camera.transform.position;
			pos.y -= rotY / 20;
			camera.transform.position = pos;
		}
	}
}
