//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

    [SerializeField]
    private Camera cam; //

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float cameraRotationX = 0f;
    private float currentCameraRottionX = 0f;
    private Vector3 thrusterForce = Vector3.zero;

    [SerializeField]
    private float cameraRotationLimit = 85f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    //Gets a movement vector
    public void Move (Vector3 _velocity)
    {
        velocity = _velocity;
    }

    //Gets a rotational vector
    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    //Gets a rotational vector for the camera
    public void RotateCamera(float  _cameraRotationX)
    {
        cameraRotationX = _cameraRotationX;
    }

    // Get a force vector for thrusters
    public void ApplyThrusterForce  (Vector3 _thrusterForce)
    {
        thrusterForce = _thrusterForce;
    }

    // Run every physics iteration
    void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }
    //Perform movement based on velocity variable
    void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);

        }
        //for thrusterforce
        if (thrusterForce != Vector3.zero)
        {
            rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration); //forcemode.acceleration adds a continuous force ignoring the mass of the body
        }
    }
    //Perform rotation 
    void PerformRotation()
    {
       
            rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation)); //Quaternion is a math function that takes vector 3 and transform into quaternions
            
            if (cam != null)
        {
            //Set the rotation and clamp it
            currentCameraRottionX -= cameraRotationX;
            currentCameraRottionX = Mathf.Clamp(currentCameraRottionX, -cameraRotationLimit, cameraRotationLimit);
            //Apply rotation to the transform of the camera
            cam.transform.localEulerAngles = new Vector3( currentCameraRottionX, 0f, 0f);
        }
    }
}
