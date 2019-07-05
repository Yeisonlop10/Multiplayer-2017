//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HostGame : MonoBehaviour {

    [SerializeField] //Will be lbeled as serialized to give permission to the usser to choose the number of players
	private uint roomSize = 6;//This will store the amount of players that can join in the room
    private string roomName;

    private NetworkManager networkManager; //Variable to Reference to network manager

    void Start()
    {
        networkManager = NetworkManager.singleton; 
        if(networkManager.matchMaker == null)//To make sure matchmaker is enabled
        {
            networkManager.StartMatchMaker();
        }
    }

    public void SetRoomName (string _name)//Function to set the room name
    {
        roomName = _name;
    }

    public void CreateRoom()//Method to create room
    {
        if(roomName != "" && roomName != null)
        {
            Debug.Log("Creating Room " + roomName + " with room for " + roomSize + " players.");
            networkManager.matchMaker.CreateMatch(roomName, roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);//create room with reference to network manager
            
        }
    }
}
