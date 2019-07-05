//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;


//In this code, some methods are going to be reused by other code files in the main progrm.

public class Util {

    public static void SetLayerRecursively (GameObject _obj, int _newLayer)
    {
        if (_obj == null)
            return;

        _obj.layer = _newLayer;

        foreach(Transform _child in _obj.transform) //Loop that goes inside all the children in the object
        {
            if (_child == null) // If child is null, then continues to the next child.
                continue;

            SetLayerRecursively(_child.gameObject, _newLayer);
        }
    }
	
}
