//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserAccountLobby : MonoBehaviour {

    public Text usernameText;

    void Start()
    {
        if(UserAccountManager.isLoggedIn)
        usernameText.text = UserAccountManager.playerUsername;
    }

    public void SignOut()
    {
        if (UserAccountManager.isLoggedIn)
            UserAccountManager.instance.LogOut();
    }
}
