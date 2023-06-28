using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{

    public SceneInteraction sceneInteraction;
    private InventoryController inventoryController;
    private PlayerController playerController;

    [Header("GAME")]
    public GameStates currentStates;

    [Header("PLAYER SETTINGS")]
    public int idPerson;
    public int idPersonCurrent;
    public int idWeaponPerson;
    public int idWeaponPersonCurrent;
    public int idArrowEquipment;

    [Header("PLAYER HEALTH")]
    public int playerMaxHealth; // Max health player can have and current health
    public int playerCurrentHealth; // Current player health

    [Header("PLAYER MANA")]
    public int playerMaxMana; // Max health player can have and current mana
    public int playerCurrentMana; // Current player mana

    [Header("PLAYER ARROWS")]
    public GameObject[] arrowsPrefab; // 0: Arrow normal ; 1: Arrow freeze ; 2: Arrow fire
    public Sprite[] arrowsImages; // 0: Arrow normal ; 1: Arrow freeze ; 2: Arrow gold
    public Sprite[] arrowsIcons; // 0: Arrow normal ; 1: Arrow freeze ; 2: Arrow fire
    public int[] arrowsVelocity; // 0: Arrow normal ; 1: Arrow fast ; 2: Arrow super fast
    public int[] arrowsQuantity; // 0: Arrow normal ; 1: Arrow freeze ; 2: Arrow fire

    [Header("PLAYER HEALTH")]
    public int[] potionsQuantity; // 0: Potion health ; 1: Potion mana

    [Header("PLAYER DB")]
    public string[] personNames;
    public Texture[] personSprites;
    public int[] personClassId;
    public GameObject[] personInitialWeapon;
    public int personInitialWeaponId;

    [Header("WEAPON SETTINGS")]
    public string[] weaponDamageType;
    public GameObject[] weaponAttackEffects;

    [Header("WEAPON DB")]
    public Sprite[] weaponsStage1; // First stage of weapon animation
    public Sprite[] weaponsStage2; // Second stage of weapon animation
    public Sprite[] weaponsStage3; // Third stage of weapon animation
    public Sprite[] weaponsStage4; // Four stage of weapon animation

    public int[] weaponClasses; // Types of weapons, including combat (0), bows (1) and staffs (2)
    public int[] weaponImprovements; // Weapon improvements

    public string[] weaponEffectsNames;
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
    public GameObject panelDead;
    public GameObject panelGameOver;

    [Header("FIRST SELECT MENU ITEM")]
    public Button firstItemSelectPanelPause;
    public Button firstItemSelectPanelItems;
    public Button firstItemSelectPanelWeapon;
    public Button firstItemSelectPanelDead;
    public Button firstItemSelectPanelGameOver;

    [Header("MATERIALS LIGHT 2D")]
    public Material defaultMaterial;
    public Material lightMaterial;

    void Start()
    {
        playerController = FindObjectOfType(typeof(PlayerController)) as PlayerController;
        inventoryController = FindObjectOfType(typeof(InventoryController)) as InventoryController;

        idPerson = PlayerPrefs.GetInt("idPerson"); // Get player selected in menu

        DontDestroyOnLoad(gameObject); // Dont destroy game controller when scene changes

        initialHealthMana(); // Set player initial health and mana
        initialInventory(); // Set player initial inventory

        hideMenus(); // Hide all panels at game start
    }

    void Update()
    {
        if (currentStates != GameStates.DEAD)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                menuPause();
            }
            
            if (Input.GetButtonDown("Inventory"))
            {
                menuItems();
            }
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

    void initialHealthMana()
    {
        playerCurrentHealth = playerMaxHealth;
        playerCurrentMana = playerMaxMana;
    }

    void initialInventory()
    {
        GameObject weapon = Instantiate(personInitialWeapon[idPerson]);

        inventoryController.items.Add(personInitialWeapon[idPerson]);
        inventoryController.itemsLoaded.Add(weapon);

        personInitialWeaponId = weapon.GetComponent<ItemController>().idItem;
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
            idWeaponPerson = personInitialWeaponId;
        }
    }

    public void gameStates(GameStates newStates)
    {
        currentStates = newStates;

        if (currentStates == GameStates.PLAY)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }

    void updateCoins()
    {
        playerCoinsGui.text = playerCoins.ToString("N0").Replace(",", "."); // No decimal case and replace "," to ".", example: "2.000"
    }

    public void menuPause()
    {
        bool panelStates = !panelPause.activeSelf;

        panelItems.SetActive(false); // Disable items menu
        panelPause.SetActive(panelStates);

        switch (panelStates)
        {
            case true:
                firstItemSelectPanelPause.Select(); // Select first button on panel
                gameStates(GameStates.PAUSE); // Set game states to pause
                break;

            case false:
                gameStates(GameStates.PLAY); // Set game states to play
                break;
        }
    }

    public void menuItems()
    {
        bool panelStates = !panelItems.activeSelf;

        panelPause.SetActive(false); // Disable pause menu
        panelItems.SetActive(panelStates); // Active items menu

        switch (panelStates)
        {
            case true:
                firstItemSelectPanelItems.Select(); // Select first button on panel
                gameStates(GameStates.INVENTORY); // Set game states to inventory
                inventoryController.loadInventory(); // Load inventory
                break;

            case false:
                gameStates(GameStates.PLAY); // Set game states to play
                break;
        }        
    }

    public void menuDead()
    {
        closeAllPanels(); // Close all panels

        panelDead.SetActive(true);

        firstItemSelectPanelDead.Select(); // Select the first item in menu
    }

    public void menuGameOver()
    {
        closeAllPanels(); // Close all panels

        panelGameOver.SetActive(true);

        firstItemSelectPanelGameOver.Select(); // Select the first item in menu
    }

    public void exitGame()
    {
        closeAllPanels(); // Close all panels

        gameStates(GameStates.PLAY); // Resume game
        
        sceneInteraction.interaction(); // Exit game
    }

    public void menuItemInfo()
    {
        panelWeapon.SetActive(true); // Active weapon menu
    }

    public void closePanels()
    {
        panelPause.SetActive(true); // Active pause menu
        panelItems.SetActive(false); // Disable menu items

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

    public void closeAllPanels()
    {
        panelPause.SetActive(false); // Disable menu pause
        panelItems.SetActive(false); // Disable menu items
        panelWeapon.SetActive(false); // Disable menu weapon
        panelDead.SetActive(false); // Disable menu dead

        gameStates(GameStates.PLAY); // Set game states to PLAY
    }

    public void removeItemFromInventory(int idSlot)
    {
        inventoryController.items.RemoveAt(idSlot);
        inventoryController.loadInventory();
        closePanelItemInfo();
    }

    public void upgradeWeapon(int idWeapon)
    {
        int upgrade = weaponImprovements[idWeapon];

        if (upgrade < 5)
        {
            upgrade += 1; // Increase weapon improvement

            weaponImprovements[idWeapon] = upgrade; // Add new upgrade
        }
    }

    public void swapItemSelect(int idSlot)
    {
        GameObject oldItem = inventoryController.items[0];
        GameObject newItem = inventoryController.items[idSlot];

        inventoryController.items[0] = newItem;
        inventoryController.items[idSlot] = oldItem;
    }

    public void gatherItem(GameObject item)
    {
        inventoryController.items.Add(item);
    }

    public void usePotion(int idPotion)
    {
        if (potionsQuantity[idPotion] <= 0) return;
        
        potionsQuantity[idPotion] -= 1; // Reduce the number of potions
            
        switch (idPotion)
        {
            case 0: // Health potion
                playerCurrentHealth += 5; // Increase player health by 2

                if (playerCurrentHealth > playerMaxHealth)
                {
                    playerCurrentHealth = playerMaxHealth; // Reduce player health to the maximum if greater than maximum
                }
                break;

            case 1: // Mana potion
                playerCurrentMana += 5; // Increase player mana by 4

                if (playerCurrentMana > playerMaxMana)
                {
                    playerCurrentMana = playerMaxMana; // Reduce player mana to the maximum if greater than maximum
                }
                break;
        }
    }
}