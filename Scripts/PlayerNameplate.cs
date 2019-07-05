//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameplate : MonoBehaviour {

    [SerializeField]
    private Text usernameText; // reference to name text over the player

    [SerializeField]
    private RectTransform healthBarFill; // reference to healthbar over the player

    [SerializeField]
    private Player player; // reference to the actual player

	// Update is called once per frame
	void Update ()
    {
        usernameText.text = player.username; //changes the text with the player name
        healthBarFill.localScale = new Vector3(player.GetHealthPct(), 1f, 1f); // dynamically changes the bar status over the player
	}
}
