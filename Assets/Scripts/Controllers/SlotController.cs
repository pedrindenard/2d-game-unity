using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotController : MonoBehaviour
{

    private GameController gameController;
    
    public GameObject objectSlot;

    void Start()
    {
        gameController = FindObjectOfType(typeof(GameController)) as GameController;
    }

    public void useItem()
    {
        if (objectSlot != null)
        {
            //objectSlot.SendMessage("useItem", SendMessageOptions.DontRequireReceiver);
            gameController.menuItemInfo();
        }
    }
}
