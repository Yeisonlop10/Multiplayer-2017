using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class killfeedItem : MonoBehaviour {

    [SerializeField]
    Text text;

    public void Setup (string player, string source)
    {
        text.text = "<b>" + source + "</b>" + " killed " + "<i>" + player + "</i>";
    }


}
