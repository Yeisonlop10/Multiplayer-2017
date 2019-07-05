//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class PauseGame : MonoBehaviour {

    public static bool isOn = false; //Variable to store pausemenu state

    private NetworkManager networkManager;//Temporary reference to NetworkManager

    void Start()
    {
        networkManager = NetworkManager.singleton;
    }

    public void LeaveRoom () //Method to leave room
    {
        MatchInfo matchInfo = networkManager.matchInfo;
        networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);//This instruction tells 
        //network manager to end the conection of the client.
        networkManager.StopHost();
    }
}
