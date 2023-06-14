using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotController : MonoBehaviour
{

    private GameController gameController;
    private WeaponInfoController weaponInfoController;
    
    public GameObject objectSlot;
    public int idSlot;

    void Start()
    {
        gameController = FindObjectOfType(typeof(GameController)) as GameController;
        weaponInfoController = FindObjectOfType(typeof(WeaponInfoController)) as WeaponInfoController;
    }

    public void useItem()
    {
        if (objectSlot != null)
        {
            weaponInfoController.objectSlot = objectSlot;
            weaponInfoController.idSlot = idSlot;

            weaponInfoController.loadWeaponInformation(); // Load the weapon information

            gameController.menuItemInfo();
        }
    }
}
