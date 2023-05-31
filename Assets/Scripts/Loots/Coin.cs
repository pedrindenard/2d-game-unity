using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    
    private GameController gameController;

    public int coinValue;

    void Start()
    {
        gameController = FindObjectOfType(typeof(GameController)) as GameController;
    }

    public void gather()
    {
        gameController.playerCoins += coinValue; // Add + coinValue to player inventory
        Destroy(gameObject); // After coin being collected, destroy from scene
    }

}