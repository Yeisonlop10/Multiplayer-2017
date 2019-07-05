using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour {

    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private Transform weaponHolder;

    [SerializeField]
    private PlayerWeapon primaryWeapon;

    private PlayerWeapon currentWeapon; //Not serialized because is not desired to change this on inspector

    private WeaponGraphics currentGraphics;

    public bool isReloading = false;//this boolean will verify the reloading state, so it doesnt happen more than once

    // Use this for initialization
	void Start ()
    {
        EquipWeapon(primaryWeapon);
	}
	
    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }

    void EquipWeapon (PlayerWeapon _weapon)
    {
        currentWeapon = _weapon;

        GameObject _weaponIns = (GameObject)Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        _weaponIns.transform.SetParent(weaponHolder);

        currentGraphics = _weaponIns.GetComponent<WeaponGraphics>();//Looking for the weapon graphics in the weapon instance
        if (currentGraphics == null)
            Debug.LogError("No weapon graphics component on the weapon object: " + _weaponIns.name);

        if (isLocalPlayer)
            Util.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));
    }
	
    public void Reload()
    {
        if (isReloading)
            return;

        StartCoroutine(Reload_Coroutine());
    }

    private IEnumerator Reload_Coroutine ()
    {
        Debug.Log("Reloading bullets...");

        isReloading = true;
        CmdOnReload();

        yield return new WaitForSeconds(currentWeapon.reloadTime);//adds the 2 seconds of reload time set in playerweapon.cs

        currentWeapon.bullets = currentWeapon.maxBullets;//reasigns max number of bullets to currentweapon.bullets

        isReloading = false;
    }

    [Command]
    void CmdOnReload()
    {
        RpcOnReload();
    }
    [ClientRpc]
    void RpcOnReload()
    {
       Animator anim =  currentGraphics.GetComponent<Animator>();// Getscomponent of the type animator in currentGraphics
        if(anim != null)
        {
            anim.SetTrigger("Reload");//Enables the trigger that allows the weapon reload animation
        }
    }
}
