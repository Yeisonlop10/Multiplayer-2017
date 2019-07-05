//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;

public class RoomListItem : MonoBehaviour {

    //way to pass from join game the associated room so the name can be changed

    public delegate void JoinRoomDelegate(MatchInfoSnapshot _match);//Delegate that contains references to setup functions. 
    //Those functions are able to subscribe to this callback (delegate). That in order to allow the functions to be called back later.
    private JoinRoomDelegate joinRoomCallback; //instance of the delegate

    [SerializeField]
    private Text roomNameText;

    private MatchInfoSnapshot match;

    public void Setup(MatchInfoSnapshot _match, JoinRoomDelegate _joinRoomCallbak)
    {

       // Debug.Log("TEST setup method");

        match = _match;
        joinRoomCallback = _joinRoomCallbak;

        roomNameText.text = match.name + " (" + match.currentSize + " / " + match.maxSize + " )";
    }

    public void JoinRoom()
    {
        joinRoomCallback.Invoke(match);//invoking the joinroomcallback with the function called
    }
}
