using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{

    private GameController gameController;

    public int idItem;
    
    void Start()
    {
        gameController = FindObjectOfType(typeof(GameController)) as GameController;
    }

    void useItem()
    {
        gameController.useItemWeapon(idItem); // Get this item by id and use it as weapon
    }
}
