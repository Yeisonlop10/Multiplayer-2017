using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerScore : MonoBehaviour {


    int lastKills = 0;
    int lastDeaths = 0;

    Player player;

    void Start()
    {
        player = GetComponent<Player>();
        StartCoroutine(SyncScoreLoop());
    }

    void OnDestroy ()
    {
        if (player != null)
        SyncNow();
    }

    IEnumerator SyncScoreLoop ()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            SyncNow();
        }

    }

    void SyncNow()
    {
        if (UserAccountManager.isLoggedIn)
        {
            UserAccountManager.instance.GetData(OnDataReceived);
        }
    }

    void OnDataReceived(string data)
    {
        if (player.kills <= lastKills && player.deaths <= lastDeaths)
            return;
        int killsSinceLast = player.kills - lastKills;
        int deathsSinceLast = player.deaths - lastDeaths;

        int kills = DataTranslator.DataToKills(data);
        int deaths = DataTranslator.DataToDeaths(data);

        //for new kills and deaths
        int newKills = killsSinceLast + kills;
        int newDeaths = deathsSinceLast + deaths;

        string newData = DataTranslator.ValuesToData(newKills, newDeaths);//storages newkills, newdata in string called newData 
                                                                          //to be sent to valuestodata

        Debug.Log("Syncing: " + newData);

        //To update kills and deaths variables
        lastKills =  player.kills;
        lastDeaths =  player.deaths;

        UserAccountManager.instance.SendData(newData);//sends updated info to valuestodata method inside datatranslator to transform it to 
        //data format by using senddata method inside useraccountmanager
    }
}
