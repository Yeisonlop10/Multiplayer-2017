using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerSetup))]

public class Player : NetworkBehaviour {

    [SyncVar] //everytime the value changes it is pushed out over all the clients
    private bool _isDead = false;//Boolean which says if the player is currently alive or not
    public bool isDead //Method to access _isDead variable
    {
        get { return _isDead; } //Anything that has reference to this instance can check if its dead or not
        protected set { _isDead = value; } //Adding protected to set makes sure that classes that derive from the player class are able to change this variable
    }



    [SerializeField]
    private int maxHealth = 100;

    [SyncVar] //everytime the value changes it is pushed out over all the clients
    private int currentHealth;

    public float GetHealthPct()
    {
        return (float)currentHealth / maxHealth; //returns the percentage value of health to use it to size the health bar
    }

    [SyncVar]
    public string username = "Loading...";

    //Variables to storage kills and deaths
    public int kills;
    public int deaths;

    [SerializeField]
    private Behaviour[] disableOnDeath; //Behaviour array  to put components to desable on death
    private bool[] wasEnabled;//Bool array to save compoents that were enabled

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;  //Array to disable components inside gameobject


    [SerializeField]
    private GameObject deathEffect; //Death Effect reference

    [SerializeField]
    private GameObject spawnEffect; //Respawn Effect reference

    private bool firstSetup = true;

    public void SetupPlayer()
    {
        if(isLocalPlayer)
        {
            //To switch the cameras just on the local player and no on the others
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }
        
        CmdBradCastNewPlayerSetup();
    }

    [Command]//calling rpc client from the server
    private void CmdBradCastNewPlayerSetup ()
    {   
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]//This method is used to indicate all the current players that a new one is spawned
    private void RpcSetupPlayerOnAllClients ()
    {
      if(firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }

            firstSetup = false;
        }

        SetDefaults();
    }

    //void Update() //this method is disabled, but is enabled for test version if want to try the killing methods work, in that case
    //              //just uncomment it all, save it, run the game and press k to kill the player and check on inspector that all the settings are disabled.
    //{
    //    if (!isLocalPlayer)
    //        return;

    //    if (Input.GetKeyDown(KeyCode.K))
    //    {
    //        RpcTakeDamage(99999);//When K is pressed it ads 99999 damage to the player.
    //    }
    //}

    //to check and show damage in all clients with APC call, a method is called in all the clients
    [ClientRpc]
    public void RpcTakeDamage (int _amount, string _sourceID)//Everytime player receives damage TakeDamage is updated with Rpc behind to help the clientrpc works on every client
    {
        if (isDead)//This makes sure to take damage only if player is not dead
            return; 
        
        currentHealth -= _amount;

        Debug.Log(transform.name + "now has " + currentHealth + " health.");

        if(currentHealth <=0)
        {
            
            Die(_sourceID);//Call to Die method

        }
    }

    private void Die(string _sourceID)//Die method
    {
        isDead = true;
        Player sourcePlayer = GameManager.GetPlayer(_sourceID);
        if(sourcePlayer != null)
        {
            sourcePlayer.kills++; //increments the number of kills of the player source
            GameManager.instance.onPlayerKilledCallback.Invoke(username, sourcePlayer.username);//instantiates and calls onplayerkilledcallback inside
            //gamemanager to send username who killes andsourceplayer who dies
            
        }

                deaths++; //increase the number of deaths

        for (int i = 0; i < disableOnDeath.Length; i++)//Disable components on the player object, so player cannot move or collide or shoot
        {
            disableOnDeath[i].enabled = false;
        }

        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)//Disable GameObjects
        {
            disableGameObjectsOnDeath[i].SetActive(false);//SetActive is boolean, so, false value is assigned
        }

        //Disabling the collider , if present
        Collider _col = GetComponent<Collider>(); 
        if (_col != null) // If collider is not equal to null
            _col.enabled = false;

        //Spawn a death Effect
        GameObject _gfxIns = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);//Quaternion is for rotation, in this case it doesnt matter, so, by applying .identity
        // rotation equals to (0,0,0)
        Destroy(_gfxIns, 3f);//Destroys the object after it executes the destroy animation after 3 seconds.

        //This must be executed for local player because its camera will be the only to be disabled when he dies.
        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        Debug.Log(transform.name + " is DEAD!");

        StartCoroutine(Respawn());//Call Respawn Method


    }

    private IEnumerator Respawn() //This method is the respawn
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime); //references to gamemanager instance match settings respawn time 

        

        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();   //Finds the singleton whichs is the instance of the network manager in the scene
        //It returns one of the start points
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);//Delay to give time to move the plaer before adding particles.

        SetupPlayer();//call to set PlayerSetup method

        Debug.Log(transform.name +" Respawned.");
    }
    

    public void SetDefaults ()
    {
        isDead = false;
        
        currentHealth = maxHealth;

        //Looping trhough the array to re enable components  as the original version of each.
        for (int i = 0; i < disableOnDeath.Length; i++)   
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        //Looping trhough the GameObject array to re enable as the original version of each component
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        //To enable collider 
        Collider _col = GetComponent<Collider>();
        if (_col != null) // If collider is not equal to null
            _col.enabled = true;

     
        //Create spawn Effect
        //Spawn a death Effect
        GameObject _gfxIns = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);//Quaternion is for rotation, in this case it doesnt matter, so, by applying .identity
        // rotation equals to (0,0,0)
        Destroy(_gfxIns, 3f);//Destroys the object after it executes the spawn animation after 3 seconds.
    }
}
