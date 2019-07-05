using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFacingBilboard : MonoBehaviour {

	
	// Update is called once per frame
	void Update () {

        Camera cam = Camera.main; //reference to the camera

        //This function will move the canvas over the player to face the camera which is pointing to it
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
            cam.transform.rotation * Vector3.up);
	}
}
