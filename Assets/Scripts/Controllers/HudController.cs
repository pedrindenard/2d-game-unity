using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{

    [Header("Controllers")]
    public PlayerController playerController;

    [Header("Health bar")]
    public Image[] playerHealth;

    [Header("Health half and full")]
    public Sprite healthHalf;
    public Sprite healthFull;

    void Start()
    {
        playerController = FindObjectOfType(typeof(PlayerController)) as PlayerController;
    }

    void Update()
    {
        updatePlayerHealth();
    }

    void updatePlayerHealth()
    {
        float percentageHealth = (float)playerController.playerCurrentHealth / (float)playerController.playerMaxHealth;

        // Current player health is 100%
        foreach (Image image in playerHealth)
        {
            image.sprite = healthFull; // Set sprite image to full health
            image.enabled = true; // Enable sprite
        }

        updateCurrentPlayerHealth(percentageHealth);
    }

    void updateCurrentPlayerHealth(float percentageHealth)
    {
        if (percentageHealth == 1)
        {
            
        }
        else if (percentageHealth >= 0.9F)
        {
            playerHealth[4].sprite = healthHalf;
        }
        else if (percentageHealth >= 0.8F)
        {
            playerHealth[4].enabled = false;
        }
        else if (percentageHealth >= 0.7F)
        {
            playerHealth[4].enabled = false;
            playerHealth[3].sprite = healthHalf;
        }
        else if (percentageHealth >= 0.6F)
        {
            playerHealth[4].enabled = false;
            playerHealth[3].enabled = false;
        }
        else if (percentageHealth >= 0.5F)
        {
            playerHealth[4].enabled = false;
            playerHealth[3].enabled = false;
            playerHealth[2].sprite = healthHalf;
        }
        else if (percentageHealth >= 0.4F)
        {
            playerHealth[4].enabled = false;
            playerHealth[3].enabled = false;
            playerHealth[2].enabled = false;
        }
        else if (percentageHealth >= 0.3F)
        {
            playerHealth[4].enabled = false;
            playerHealth[3].enabled = false;
            playerHealth[2].enabled = false;
            playerHealth[1].sprite = healthHalf;
        }
        else if (percentageHealth >= 0.2F)
        {
            playerHealth[4].enabled = false;
            playerHealth[3].enabled = false;
            playerHealth[2].enabled = false;
            playerHealth[1].enabled = false;
        }
        else if (percentageHealth >= 0.1F)
        {
            playerHealth[4].enabled = false;
            playerHealth[3].enabled = false;
            playerHealth[2].enabled = false;
            playerHealth[1].enabled = false;
            playerHealth[0].sprite = healthHalf;
        }
        else if (percentageHealth >= 0.0F)
        {
            playerHealth[4].enabled = false;
            playerHealth[3].enabled = false;
            playerHealth[2].enabled = false;
            playerHealth[1].enabled = false;
            playerHealth[0].enabled = false;
        }
    }
}
