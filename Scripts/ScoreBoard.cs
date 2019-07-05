using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard : MonoBehaviour {

    [SerializeField]
    GameObject playerScoreBoardItem; //Reference to scoreboarditem

    [SerializeField]
    Transform playerScoreBoardList;

	void OnEnable()//this will be called when enabled through the UI
    {
        //Get an array of players
        Player[] players = GameManager.GetAllPlayers();
        
        //loop through and set up a list item for each one
        foreach(Player player in players)
        {
           GameObject itemGO = (GameObject)Instantiate(playerScoreBoardItem, playerScoreBoardList);
            Debug.Log(player.username + " | " + player.kills + " | " + player.deaths);
            PlayerScoreBoardItem item = itemGO.GetComponent<PlayerScoreBoardItem>(); 
            if(item != null)
            {
                item.Setup(player.username, player.kills, player.deaths);
            }
        }
        
    }

    void OnDisable ()
    {
        //Clean up the list of items
        foreach(Transform child in playerScoreBoardList)
        //to loop inside the object and its childre, transfor is used because every object and children have a transform
        {
            Destroy(child.gameObject);
        }
    }
}
