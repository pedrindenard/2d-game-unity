using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    
    private GameController gameController;

    public int arrowId;
    public int arrowAmount;

    public bool gathered;

    void Start()
    {
        gameController = FindObjectOfType(typeof(GameController)) as GameController;
    }

    public void gather()
    {
        if (!gathered)
        {
            gameController.arrowsQuantity[arrowId] += arrowAmount;
        }

        gathered = true; // Item gathered
        
        Destroy(gameObject); // After coin being collected, destroy from scene
    }
}