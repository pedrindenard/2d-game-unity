using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{

    // Transition animations
    public FadeInController fadeController;

    // Attack damage percentage
    public string[] weaponDamageType;

    // Attack effects
    public GameObject[] weaponAttackEffects;

    // Player coins
    public TextMeshProUGUI playerCoinsGui;
    public int playerCoins;

    void Start()
    {
        fadeController = FindObjectOfType(typeof(FadeInController)) as FadeInController;
        fadeController.fadeOut();
    }

    void Update()
    {
        updatePlayerCoins();
    }

    void updatePlayerCoins()
    {
        playerCoinsGui.text = playerCoins.ToString("N0").Replace(",", "."); // No decimal case and replace "," to ".", example: "2.000"
    }
}