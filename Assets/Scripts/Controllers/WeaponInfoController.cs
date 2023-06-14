using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponInfoController : MonoBehaviour
{

    private GameController gameController;
    private int improvement;
    private int weaponId;

    [Header("ITEM")]
    public GameObject objectSlot;
    public int idSlot;

    [Header("VIEWS")]
    public Image imageItem;
    public TextMeshProUGUI nameItem;
    public TextMeshProUGUI damageItem;
    
    [Header("UPGRADES")]
    public GameObject[] upgrades;

    [Header("BUTTONS")]
    public Button upgradeButton;
    public Button equipButton;
    public Button excludeButton;

    void Start()
    {
        gameController = FindObjectOfType(typeof(GameController)) as GameController;
    }

    public void loadWeaponInformation()
    {
        ItemController itemController = objectSlot.GetComponent<ItemController>(); // Get weapon information

        weaponId = itemController.idItem; // Set weapon id at script scope

        string damageType = gameController.weaponDamageType[gameController.damageEffect[weaponId]];

        int damageMin = gameController.minDamage[weaponId]; // Min damage
        int damageMax = gameController.maxDamage[weaponId]; // Max damage

        imageItem.sprite = gameController.weaponImages[weaponId]; // Set weapon icon

        damageItem.text = "Damage: " + damageMin + "-" + damageMax + " / " + damageType; // Set weapon description
        nameItem.text = gameController.weaponNames[weaponId]; // Set weapon name

        loadUpgrades();

        interactableButtons();
    }

    public void interactableButtons()
    {
        int idClassWeapon = gameController.weaponClasses[weaponId];
        int idClassPerson = gameController.personClassId[gameController.idPerson];

        // Disable slot selection and exclude for first slot (First slot is item selected)
        // and if player class is != from class of weapon
        if (idSlot == 0)
        {
            equipButton.interactable = false;
            excludeButton.interactable = false;
        }
        else
        {
            equipButton.interactable = idClassPerson == idClassWeapon;
            excludeButton.interactable = idClassPerson == idClassWeapon;
        }
    }

    public void loadUpgrades()
    {
        improvement = gameController.weaponImprovements[weaponId];

        upgradeButton.interactable = improvement < 5;

        foreach (GameObject upgrade in upgrades)
        {
            upgrade.SetActive(false);
        }

        for (int index = 0; index < improvement; index++)
        {
            upgrades[index].SetActive(true);
        }
    }

    public void upgradeWeapon()
    {
        gameController.upgradeWeapon(weaponId); // Upgrade weapon
        loadUpgrades(); // Update screen
    }

    public void equipWeapon()
    {
        objectSlot.SendMessage("useItem", SendMessageOptions.DontRequireReceiver); // Equip item
        gameController.swapItemSelect(idSlot); // Swap item selected with first item
        gameController.closeAllPanels(); // Back to game play state
    }

    public void removeWeapon()
    {
        gameController.removeItemFromInventory(idSlot);
    }
}