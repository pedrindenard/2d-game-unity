using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{

    private InventoryController inventoryController;
    private PlayerController playerController;

    [Header("GAME")]
    public GameStates currentStates;

    [Header("PLAYER SETTINGS")]
    public int idPerson;
    public int idPersonCurrent;
    public int maxLifePerson;
    public int idWeaponPerson;
    public int idWeaponPersonCurrent;

    [Header("PLAYER DB")]
    public string[] personNames;
    public Texture[] personSprites;
    public int[] personClassId;
    public int[] personInitialWeaponId;

    [Header("WEAPON SETTINGS")]
    public string[] weaponDamageType;
    public GameObject[] weaponAttackEffects;

    [Header("WEAPON DB")]
    public Sprite[] weaponsStage1; // First stage of weapon animation
    public Sprite[] weaponsStage2; // Second stage of weapon animation
    public Sprite[] weaponsStage3; // Third stage of weapon animation
    public Sprite[] weaponsStage4; // Four stage of weapon animation

    public int[] weaponClasses; // Types of weapons, including combat (0), bows (1) and staffs (2)

    public string[] weaponNames;
    public Sprite[] weaponImages;

    public int[] damageEffect;
    public int[] maxDamage;
    public int[] minDamage;

    public int[] cost;

    [Header("INVENTORY")]
    public TextMeshProUGUI playerCoinsGui;
    public int playerCoins;

    [Header("PANELS")]
    public GameObject panelPause;
    public GameObject panelItems;
    public GameObject panelWeapon;

    [Header("FIRST SELECT MENU ITEM")]
    public Button firstItemSelectPanelPause;
    public Button firstItemSelectPanelItems;
    public Button firstItemSelectPanelWeapon;

    void Start()
    {
        inventoryController = FindObjectOfType(typeof(InventoryController)) as InventoryController;
        playerController = FindObjectOfType(typeof(PlayerController)) as PlayerController;

        idPerson = PlayerPrefs.GetInt("idPerson"); // Get player selected in menu

        DontDestroyOnLoad(gameObject); // Dont destroy game controller when scene changes
        hideMenus(); // Hide all panels at game start
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            menuPause();
        }
        updateCoins();
    }

    public void updatePlayerSkin()
    {
        idPersonCurrent = idPerson;
    }

    public Sprite weaponStage(WeaponStageStates stage, int id)
    {
        switch (stage) {
            case WeaponStageStates.ONE:
                return weaponsStage1[id];

            case WeaponStageStates.TWO:
                return weaponsStage2[id];

            case WeaponStageStates.THREE:
                return weaponsStage3[id];

            default:
                return weaponsStage4[id];
        }
    }

    void hideMenus()
    {
        panelPause.SetActive(false); // Set menu pause disable at game start
        panelItems.SetActive(false); // Set menu items disable at game start
        panelWeapon.SetActive(false); // Set menu weapon disable at game start
    }

    public void validateWeapon()
    {
        if (weaponClasses[idWeaponPerson] != personClassId[idPerson])
        {
            idWeaponPerson = personInitialWeaponId[idPerson];
        }
    }

    public void gameStates(GameStates newStates)
    {
        currentStates = newStates;
    }

    void updateCoins()
    {
        playerCoinsGui.text = playerCoins.ToString("N0").Replace(",", "."); // No decimal case and replace "," to ".", example: "2.000"
    }

    public void menuPause()
    {
        if (currentStates != GameStates.INVENTORY)
        {
            bool panelStates = !panelPause.activeSelf;

            panelPause.SetActive(panelStates);

            switch (panelStates)
            {
                case true:
                    firstItemSelectPanelPause.Select(); // Select first button on panel
                    gameStates(GameStates.PAUSE); // Set game states to pause
                    Time.timeScale = 0; // Pause all times and engines
                    break;

                case false:
                    gameStates(GameStates.PLAY); // Set game states to play
                    Time.timeScale = 1; // Resume all times and engines
                    break;
            }
        }
    }

    public void menuItems()
    {
        panelPause.SetActive(false); // Disable pause menu
        panelItems.SetActive(true); // Active items menu

        firstItemSelectPanelItems.Select(); // Select first button on panel
        inventoryController.loadInventory();

        gameStates(GameStates.INVENTORY); // Set game states
    }

    public void menuOptions()
    {
        //panelPause.SetActive(false); // Disable pause menu
        //panelOptions.SetActive(true); // Active options menu

        //firstItemSelectPanelOptions.Select(); // Select first button on panel

        //gameStates(GameStates.INVENTORY); // Set game states
    }

    public void menuStatus()
    {
        //panelPause.SetActive(false); // Disable pause menu
        //panelStatus.SetActive(true); // Active status menu

        //firstItemSelectPanelStatus.Select(); // Select first button on panel

        //gameStates(GameStates.INVENTORY); // Set game states
    }

    public void menuItemInfo()
    {
        panelWeapon.SetActive(true); // Active weapon menu
    }

    public void closePanels()
    {
        panelPause.SetActive(true); // Active pause menu

        //panelStatus.SetActive(false); // Disable menu status
        panelItems.SetActive(false); // Disable menu items
        //panelOptions.SetActive(false);  // Disable menu options

        firstItemSelectPanelPause.Select(); // Select first button on panel

        inventoryController.cleanOldInventory(); // Remove gameObjects from scene

        gameStates(GameStates.PAUSE); // Set game states to PAUSE
    }

    public void closePanelItemInfo()
    {
        panelWeapon.SetActive(false); // Disable weapon menu
        firstItemSelectPanelWeapon.Select(); // Select first button on panel
    }

    public void useItemWeapon(int idWeapon)
    {
        playerController.weaponSelected(idWeapon);
    }
}