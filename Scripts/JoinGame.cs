using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class JoinGame : MonoBehaviour {

    List<GameObject> roomList = new List<GameObject>();//List of rooms(Game Objects) to refresh

    [SerializeField]
    private Text status;

    [SerializeField]
    private GameObject roomListItemPrefab; //New object to instantiate

    [SerializeField]
    private Transform roomListParent;//Reference to the scrollview


    private NetworkManager networkManager;

    void Start()
    {
        networkManager = NetworkManager.singleton;
        //Validation that matchmaker is set before using it
        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }
        RefreshRoomList();
    }

    public void RefreshRoomList() //Public method , so it can be called from the button in the gui
    {
        ClearRoomList();

        //if matchmaker is null, this makes sure to restart it
        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }

        networkManager.matchMaker.ListMatches(0, 20, "", true,0,0, OnMatchList);
        status.text = "Loading...";
    }

    public void OnMatchList (bool success, string extendedInfo, List<MatchInfoSnapshot>matches)
    {
        status.text = ""; //deletes the text in status.text
        if(!success || matches == null)
        {
            status.text = "Couldn't get room list.";
            return;
        }

        
        foreach(MatchInfoSnapshot match in matches)//Loop to add new elements based on descriptions found in "matches"
        {
            GameObject _roomListItemGO = Instantiate(roomListItemPrefab);
            _roomListItemGO.transform.SetParent(roomListParent); //Parent it to the room list parent
            RoomListItem _roomListItem = _roomListItemGO.GetComponent<RoomListItem>(); //Space to have a component sit on the gameobject that will take care of setting up the name/amount of users
            //as well as setting up a callback function that will join the game.
            if (_roomListItem != null)
            {
                _roomListItem.Setup(match, JoinRoom);
            }

            roomList.Add(_roomListItemGO);
        }

        if (roomList.Count == 0)
        {
            status.text = "No rooms at the moment.";
        }

    }

    void ClearRoomList()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);//Loop to remove objects from content
        }

        roomList.Clear();//Clearing list
    }

    public void JoinRoom(MatchInfoSnapshot _match)
    {
        Debug.Log("Joining " + _match.name);
        networkManager.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        StartCoroutine(WaitForJoin());
    }

    IEnumerator WaitForJoin () //In this function a 10 seconds timer is added to give timeout to join to the room
    {
        ClearRoomList();
        

        int countdown = 10;
        while(countdown > 0)
        {

            status.text = "Joining...(" +countdown + ")";

            yield return new WaitForSeconds(1); //1 second delay

            countdown--; //substracting from countdown

        }

        //Failed to connect
        status.text = "Failed to Connect.";
        yield return new WaitForSeconds(1); //1 second delay

        //This part of the code is also in PauseGame.cs its added also here and modified with an if statement to allow the game to end the connection 
        MatchInfo matchInfo = networkManager.matchInfo;
        if(matchInfo != null)
        {
            networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);//This instruction tells 
            //network manager to end the conection of the client.
            networkManager.StopHost();
        }
       
        RefreshRoomList();
    }

}
