using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    [SerializeField]
    RectTransform thrusterFuelFill; //Reference to the thruster fuel bar

    [SerializeField]
    RectTransform healthBarFill; //Reference to the health fill bar

    [SerializeField]
    Text ammoText; 

    [SerializeField]//Reference to the pause menu
    GameObject pauseMenu;

    [SerializeField]//Reference to the scoreboard
    GameObject scoreboard;

    private Player player;
    private PlayerController controller; //Reference to the player controller
    private WeaponManager weaponManager; //Reference to WeaponManager

    public void SetPlayer  (Player _player)
    {
        player = _player;
        controller = player.GetComponent<PlayerController>();//reference to the controller
        weaponManager = player.GetComponent<WeaponManager>();
    }

    void Start()
    {
        PauseGame.isOn = false;
    }

    void Update()
    {
        SetFuelAmount (controller.GetThrusterFuelAmount());
        SetHealthAmount(player.GetHealthPct());
        SetAmmoAmount(weaponManager.GetCurrentWeapon().bullets);

        //Pause by using escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
        //enabling scoreboard view by pressing tab key
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            scoreboard.SetActive(true);
        }else if(Input.GetKeyUp(KeyCode.Tab))
        {
            scoreboard.SetActive(false);
        }

    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);//To get the current active state of the object
        PauseGame.isOn = pauseMenu.activeSelf;
    }

    void SetFuelAmount (float _amount)
    {
        thrusterFuelFill.localScale = new Vector3(1f, _amount, 1f);
    }

    void SetHealthAmount(float _amount)
    {
        healthBarFill.localScale = new Vector3(1f, _amount, 1f);
    }

    void SetAmmoAmount(int _amount)
    {
        ammoText.text = _amount.ToString();
    }
}
