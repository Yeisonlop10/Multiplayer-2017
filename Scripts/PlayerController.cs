//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

    [SerializeField] // This makes the element show in inspector no matter if is declared as private
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 3f;

    [SerializeField]
    private float thrusterForce = 1000f;

    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f;
    [SerializeField]
    private float thrusterFuelRegenSpeed = 0.3f;
    private float thrusterFuelAmount = 1f;

    public float GetThrusterFuelAmount ()
    {
        return thrusterFuelAmount;
    }

    [SerializeField]
    private LayerMask environmentMask;

    [Header("Spring settings")]
    
    [SerializeField]
    private float jointSpring = 20;
    [SerializeField]
    private float jointMaxForce = 40f;

    //Component caching
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;

    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();

        SetJointSettings(jointSpring);
    }

    void Update()
    {
        if (PauseGame.isOn) //Pausemode on validation. if pause mode is on, player doesnt shoot or move.

        {
            if (Cursor.lockState != CursorLockMode.None)
                Cursor.lockState = CursorLockMode.None;

            motor.Move(Vector3.zero); // these 3 instructions will stay player in the same place when the game is paused
            motor.Rotate(Vector3.zero);
            motor.RotateCamera(0f);
            return;
        }
            

       if(Cursor.lockState != CursorLockMode.Locked) //When pausing the game, this will validate the cursor lock state and  disable it. So, cursor disapears when playing.

        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        RaycastHit _hit;//This part is used to keep the player with distance to the floor or any object where it stands.
        //This code will allow to get that distance by measuring with raycast hits to that object in order to get the coordinates
        if (Physics.Raycast(transform.position, Vector3.down, out _hit, 100f, environmentMask))
        {
            joint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);
        }else //if nothing is hit, then 
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }
        
        //Calculate movement velocity as a 3D vector
        float xMov = Input.GetAxis("Horizontal");
        float zMov = Input.GetAxis("Vertical");

        Vector3 movHorizontal = transform.right * xMov; 
        Vector3 movVertical = transform.forward * zMov;

        //Final movement Vector
        Vector3 velocity = (movHorizontal + movVertical) * speed; // After multiplying this function normalizes the movement

        //Animate movement
        animator.SetFloat("ForwardVelocity", zMov);


        //Apply movement
        motor.Move(velocity);

        //calculate rotation as a 3D vector (Turning around)
        float yRot = Input.GetAxisRaw("Mouse X"); // to get input from mouse

        Vector3 rotation = new Vector3(0f, yRot, 0f) * lookSensitivity; // Just to affect movement on the X axis, other are not necessary here unless anothe type of details is desired to be shown

        //Apply rotation
        motor.Rotate(rotation);


        //calculate camera rotation as a 3D Vector
        float xRot = Input.GetAxisRaw("Mouse Y"); // to get input from mouse

        float cameraRotationX = xRot * lookSensitivity; // Just to affect movement on the X axis, other are not necessary here unless anothe type of details is desired to be shown


        //Apply camera rotation
        motor.RotateCamera(cameraRotationX);

        //Code for calculate forthruster force bsed on player input
        Vector3 _thrusterForce = Vector3.zero;
        
        if (Input.GetButton("Jump") && thrusterFuelAmount > 0f)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime; // formula to reduce fuel amount

            _thrusterForce = Vector3.up * thrusterForce;
            SetJointSettings(0f);
        }
        else
        {
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime; //formula to increment fuel amount

            SetJointSettings(jointSpring);
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f); //Clamp limits the value between 0 and 1.

        //Apply thruster force
        motor.ApplyThrusterForce(_thrusterForce);
    }

    private void SetJointSettings (float _jointSpring)
    {
        joint.yDrive = new JointDrive
        {
            
            positionSpring = _jointSpring,
            maximumForce = jointMaxForce
        };
    }
}
