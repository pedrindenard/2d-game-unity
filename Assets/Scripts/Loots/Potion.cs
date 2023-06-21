using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    
    private GameController gameController;

    public int potionId;
    public int potionAmount;

    public bool gathered;

    void Start()
    {
        gameController = FindObjectOfType(typeof(GameController)) as GameController;
    }

    public void gather()
    {
        if (!gathered)
        {
            gameController.potionsQuantity[potionId] += potionAmount;
        }

        gathered = true; // Item gathered

        Destroy(gameObject); // After potion being collected, destroy from scene
    }
}