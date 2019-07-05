//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; //unityengine.netwoking must be used


[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]

public class PlayerSetup : NetworkBehaviour { //change monobehaviour for networkbehaviour to control the network actions
    [SerializeField]
    Behaviour[] componentsToDisable; //array where components will be verified to be disabled when the user spawns

    //To specify the name of the remote layer
    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    //Code to disable objects to be shown in camera
    [SerializeField]
    string dontDrawLayerName = "DontDraw";
    [SerializeField]
    GameObject playerGraphics;

    [SerializeField]
    GameObject playerUIprefab; //GameObject variable to store changes on canvas object to enable and disable when player spawns and dies.
    [HideInInspector]//To hide variable from inspector
    public GameObject playerUIInstance;


    void Start () //when the player spawns it checks if thw player is being controlled if not all the components are disabled
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {
            //Disable player graphics for local player
            SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName)); //Layermask.Nametolayer transforms to int the name of the received mask

            //Create PlayerUI
            playerUIInstance = Instantiate(playerUIprefab);
            playerUIInstance.name = playerUIprefab.name;


            //Configure PlayerUI
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            if (ui == null)
                Debug.LogError("No PlayerUI component on the PlayerUI prefab");
            ui.SetPlayer(GetComponent<Player>());

            GetComponent<Player>().SetupPlayer();

            //to setup the username on the server
            string _username = "Loading...";
            if (UserAccountManager.isLoggedIn)
                _username = UserAccountManager.playerUsername;
            else
                _username = transform.name;
                CmdSetUsername(transform.name, _username);
        }

        
    }

    [Command]
    void CmdSetUsername (string playerID, string username)
    {
        Player player = GameManager.GetPlayer(playerID);
        if (player != null)
        {
            Debug.Log(username + " has joined");
            player.username = username;
        }
    }

    //SetLayer recursively method. This type of methods must be implemented with care because can cause infinite loops and
    //the next frame would never be loaded
    void SetLayerRecursively(GameObject obj, int newLayer)//This method will change the layer of the sent object. Also will change the layer of all of the
        //child objects
    {
        obj.layer = newLayer; //To change the objects layer

        //Loop to change the child layers
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);//Calling the method again and passing the child attribute to change all the layers inside child object 
            //Good technique, but risky and takes process
        }


    }

    public override void OnStartClient() //Built method on the networkbehaviour class called everytime the client is setup locally, 
    {
        base.OnStartClient();

        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();
        GameManager.RegisterPlayer(_netID, _player);//calls registerplayer written in gamemanager.cs and gives inputs
    }

    void AssignRemoteLayer ()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName); //all layers are asigned numbers. This function takes the string and converts to the index we want and puts back to the gameobject  
    }

    void DisableComponents ()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    //When we are destroyed
    //To re enable the camera
    void OnDisable ()
    {
        //To destroy player UI
        Destroy(playerUIInstance);

        if(isLocalPlayer)
        GameManager.instance.SetSceneCameraActive(true);

        //To deregister players once they are killed
        GameManager.UnRegisterPlayer(transform.name);
    }

}
