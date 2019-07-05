using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour {

    public Text deathCount;
    public Text killCount;
    
    // Use this for initialization
	void Start () {
        if(UserAccountManager.isLoggedIn)
        UserAccountManager.instance.GetData(OnReceivedData);
	}
	
	void OnReceivedData(string data)//This is added as a function because it takes time to receive info from the server
    {
        if (killCount == null || deathCount == null)
            return;

        killCount.text = DataTranslator.DataToKills(data).ToString() + " Kills";//Calls datatokills function in datatranslator and sends data in form of string
       deathCount.text = DataTranslator.DataToDeaths(data).ToString() + " Deaths";//Calls datatodeaths function in datatranslator and sends data in form of string
    }
}
