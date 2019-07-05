using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killfeed : MonoBehaviour {


    [SerializeField]
    GameObject killfeedItemPrefab; 

    // Use this for initialization
	void Start ()
    {
        GameManager.instance.onPlayerKilledCallback += OnKill;
	}
	
	public void OnKill(string player, string source)
    {
        GameObject go =  Instantiate(killfeedItemPrefab, this.transform);
        go.GetComponent<killfeedItem>().Setup(player, source);
        //Debug.Log(source + " killed " + player);

        Destroy(go, 4f); //Destroys the go object which is the UI that shows killsfeed
    }
}
