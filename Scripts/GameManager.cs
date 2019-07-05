//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//using UnityEngine.Networking;

public class GameManager : MonoBehaviour {

    //singleton pattern allows to have only one instance of something running at any time
    public static GameManager instance;  //ID variable created to store a static reference to that instance

    public MatchSettings matchSettings; //this is configured on inspector

    [SerializeField]
    private GameObject sceneCamera; //Reference to the entire scene camera, in order to disable it, it will be disabled together with all its
    //components, audio listeners, etc.

    public delegate void OnPlayerKilledCallback(string player, string source);
    public OnPlayerKilledCallback onPlayerKilledCallback;

    void Awake()
    {
        if (instance != null) // To check if there is any other object in the scene
        {
            Debug.LogError("More than one Game Manager in scene.");
        }
        else
        {
            instance = this;
        }
    }

    public void SetSceneCameraActive (bool isActive)
    {
        if (sceneCamera == null)
            return;

        sceneCamera.SetActive(isActive);
    }


    //To register and keep track of the players
    #region Player tracking 

    private const string PLAYER_ID_PREFIX = "Player ";

    private static Dictionary<string, Player> players = new Dictionary<string, Player>();//Restricts to have only players in the dictionary


    //Methods that will interact with the dictionary
    public static void RegisterPlayer (string _netID, Player _player)//string _netID will take the player's network id component to transform it later into player id
    {
        string _playerID = PLAYER_ID_PREFIX + _netID; //normalizes the name and id in a string 
        players.Add(_playerID, _player);//Add it to the dictionary
        _player.transform.name = _playerID;//Rename the player transform to the player id

    }

    public static void UnRegisterPlayer (string _playerID)
    {
        players.Remove(_playerID); //Gets the player id and removes it from the dictionary
    }
    //Getting a player with some ID
    public static Player GetPlayer (string _playerID)
    {
        return players[_playerID];
    }

    public static Player[] GetAllPlayers ()
    {
        return players.Values.ToArray(); // Uses the players dictionary and returns all the values
        //In order to enable ToArray function, Using system.linq has to be added on top of script.
    }

    //Creating an User Interface
    // void OnGUI ()
    //{
    //    GUILayout.BeginArea(new Rect(200, 200, 200, 500)); //Rectangle area
    //    GUILayout.BeginVertical();

    //    foreach (string _playerID in players.Keys)
    //    {
    //        GUILayout.Label(_playerID + "   -  " + players[_playerID].transform.name);
    //    }

    //    GUILayout.EndVertical();
    //    GUILayout.EndArea();

    //}
#endregion
}
