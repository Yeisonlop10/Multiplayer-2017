//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; //unityengine.netwoking must be used

[RequireComponent(typeof(WeaponManager))]

public class PlayerShoot : NetworkBehaviour
{

    private const string PLAYER_TAG = "Player";
    
    

    //Reference to the player camera
    [SerializeField]
    private Camera cam;

    //In order to control what the ray hits, this layermask is created
    [SerializeField]
    private LayerMask mask;

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

     void Start()
    {
         if (cam == null)
        {
            Debug.LogError("PlayerShoot: No Camera Found");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();

    }

    void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        if (PauseGame.isOn) //Pausemode on validation
            return;

        if (currentWeapon.bullets < currentWeapon.maxBullets)
        {
            if (Input.GetButtonDown("Reload"))//Reload function if R key is pressed
            {
                weaponManager.Reload();
                return;
            }
        }

       
        if(currentWeapon.fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))
                    {
                        Shoot();
                    }
        } else
        {
            if(Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f/currentWeapon.fireRate);
            }else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
        
    }

    [Command] //Action excecuted only on the server when the player shoots
    void CmdOnShoot () //Action always called on shoot no matter if any object is hit or not
    {
        RpcDoShootEffect();
    }

    [ClientRpc]//Attribute that can be put on methods of networkbehaviour classes to allow them to be onvoked by the clients of a server.
    //Its called on all clients when the shoot effect is needed.
    void RpcDoShootEffect() //It will control the behaviour of the muzzle effect
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();//Reference to the muzzleflash attribute on weaponManager and enable to play it.
    }

    [Command]
    void CmdOnHit(Vector3 _pos, Vector3 _normal) //To call the cmdonhit action with Vector3 input parameters position(impact position) and normal(Points out straight from the surface) 
    {
        RpcDoHitEffect(_pos, _normal);
    }
    [ClientRpc]//
    void RpcDoHitEffect(Vector3 _pos, Vector3 _normal) //It will control the behaviour of the hit Effect
    {
        //Instatiation of a gameobject with the hiteffect
        GameObject _hitEffect = (GameObject) Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
        Destroy(_hitEffect, 2f);//Destroy hit effect after 2 seconds.
    }

    //This method is only called on the client side, never on server. 
    [Client]
    void Shoot ()
    {

        if(!isLocalPlayer || weaponManager.isReloading)
        {
            return;
        }


        if (currentWeapon.bullets <= 0)
        {
            Debug.Log("Out of bullets");//if bullets <0 shows log : Out of bullets
            weaponManager.Reload(); // Calls reload function inside weapon manager
            return;
        }
        currentWeapon.bullets--;
        Debug.Log("Remaining bullets: " + currentWeapon.bullets); //After every bulle substraction shows log with number of bullets

        CmdOnShoot(); //Locar player shoots , then this calls the function on the server.

        RaycastHit hit; //Raycasthit casts a ray and saves info into variable named hit
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask)) //Physics.Raycast is a vector 3 variable
        {
            //Debug.Log("We hit" + hit.collider.name);
            if (hit.collider.tag == PLAYER_TAG)  //If an element is hit by the shoot, then calls the cmdplayershot method
            {
                CmdPlayerShot(hit.collider.name, currentWeapon.damage, transform.name);
            }

            CmdOnHit(hit.point, hit.normal);//Calls the cmdonhit to reproduce the hiteffect with the vector3 coordinates.
        }

        if(currentWeapon.bullets <=0)
        {
            weaponManager.Reload();
        }
    }

    [Command] //This method is only called on the server
    void CmdPlayerShot (string playerID, int _damage, string _sourceID)
    {
        Debug.Log(playerID + " has been shot."); //it will show which element has been shot

        //Destroy (GameObject.Find(ID)); //Finds and destroys the object found by ID, but is disabled because is slow, could be applied later or in another game

        Player _player =  GameManager.GetPlayer(playerID);
        _player.RpcTakeDamage(_damage, _sourceID);
    }
}
