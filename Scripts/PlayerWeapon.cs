
using UnityEngine;
[System.Serializable] //by doing this, unity will now how to sabe and serialize this class and the variables damage and range will be present on inspector tab
public class PlayerWeapon {

    public string name = "Glock";

    public int damage = 10;
    public float range = 200f;

    
    public float fireRate = 0f;

    public int maxBullets = 20; // Max number of bullets
    [HideInInspector]//will not be shown in inspector
    public int bullets;

    public float reloadTime = 1f;

    public GameObject graphics; //weapon graphics

    public PlayerWeapon() //constructor
    {
        bullets = maxBullets;
    }
}
