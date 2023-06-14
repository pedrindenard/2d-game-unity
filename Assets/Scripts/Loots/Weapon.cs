using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    private GameController gameController;

    public GameObject[] items;
    public bool gathered;

    void Start()
    {
        gameController = FindObjectOfType(typeof(GameController)) as GameController;
    }

    public void gather()
    {
        if (!gathered)
        {
            gameController.gatherItem(items[gameController.personClassId[gameController.idPerson]]); // Gather item
        }
        
        gathered = true; // Item gathered

        Destroy(gameObject); // After coin being collected, destroy from scene
    }

}