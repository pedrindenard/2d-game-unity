using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudController : MonoBehaviour
{

    [Header("Controllers")]
    public PlayerController playerController;
    public GameController gameController;

    [Header("Health bar")]
    public Image[] playerHealth;
    public Image[] playerMana;

    [Header("Health half and full")]
    public Sprite healthHalf;
    public Sprite healthFull;

    [Header("Mana half and full")]
    public Sprite manaHalf;
    public Sprite manaFull;

    [Header("Panels")]
    public GameObject panelMana;
    public GameObject panelArrow;

    [Header("Text")]
    public TextMeshProUGUI arrowText;
    public TextMeshProUGUI potionManaText;
    public TextMeshProUGUI potionHealthText;

    [Header("Images")]
    public Image arrowImage;

    void Start()
    {
        gameController = FindObjectOfType(typeof(GameController)) as GameController;
        playerController = FindObjectOfType(typeof(PlayerController)) as PlayerController;

        checkPlayerHud(); // Enable/disable player hud`s
    }

    void Update()
    {
        updatePlayerHealth();
        updatePlayerPotions();
        
        if (panelMana.activeSelf) // Do action if mana hud is active
        {
            updatePlayerMana();
        }
        else if (panelArrow.activeSelf)
        {
            updatePlayerArrows();
        }
    }

    void checkPlayerHud()
    {
        switch (gameController.personClassId[gameController.idPerson])
        {
            case 0: // Combat
                panelMana.SetActive(false); // Disable mana hud
                panelArrow.SetActive(false); // Disable arrow hud
                break;

            case 1: // Bow
                panelArrow.SetActive(true); // Enable arrow hud
                panelMana.SetActive(false); // Disable mana hud
                updatePlayerArrows();
                break;

            case 2: // Staff
                panelMana.SetActive(true); // Enable mana hud
                panelArrow.SetActive(false); // Disable arrow hud
                break;
        }
    }

    void updatePlayerArrows()
    {
        arrowImage.sprite = gameController.arrowsIcons[gameController.idArrowEquipment];
        arrowText.text = "x" + gameController.arrowsQuantity[gameController.idArrowEquipment];

        if (Input.GetButtonDown("Arrow Left"))
        {
            nextArrow(0);
        }
        else if (Input.GetButtonDown("Arrow Right"))
        {
            nextArrow(1);
        }
    }

    void updatePlayerHealth()
    {
        float percentageHealth = (float)gameController.playerCurrentHealth / (float)gameController.playerMaxHealth;

        if (Input.GetButtonDown("Item A") && percentageHealth < 1) // Enable keyword health use
        {
            gameController.usePotion(0); // 0: Health potion
        }

        // Current player health is 100%
        foreach (Image image in playerHealth)
        {
            image.sprite = healthFull; // Set sprite image to full health
            image.enabled = true; // Enable sprite
        }

        updateCurrentPlayerHealth(percentageHealth);
    }

    void updatePlayerMana()
    {
        float percentageMana = (float)gameController.playerCurrentMana / (float)gameController.playerMaxMana;

        if (Input.GetButtonDown("Item B") && percentageMana < 1) // Enable keyword mana use
        {
            gameController.usePotion(1); // 1: Mana potion
        }

        // Current player mana is 100%
        foreach (Image image in playerMana)
        {
            image.sprite = manaFull; // Set sprite image to full mana
            image.enabled = true; // Enable sprite
        }

        updateCurrentPlayerMana(percentageMana);
    }

    void updatePlayerPotions()
    {
        potionHealthText.text = "x" + gameController.potionsQuantity[0]; // 0: Health
        potionManaText.text = "x" + gameController.potionsQuantity[1]; // 1: Mana
    }

    void updateCurrentPlayerHealth(float percentageHealth)
    {
        if (percentageHealth >= 1)
        {
            playerHealth[4].sprite = healthFull;
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

    void updateCurrentPlayerMana(float percentageMana)
    {
        if (percentageMana >= 1)
        {
            playerMana[4].sprite = manaFull;
        }
        else if (percentageMana >= 0.9F)
        {
            playerMana[4].sprite = manaHalf;
        }
        else if (percentageMana >= 0.8F)
        {
            playerMana[4].enabled = false;
        }
        else if (percentageMana >= 0.7F)
        {
            playerMana[4].enabled = false;
            playerMana[3].sprite = manaHalf;
        }
        else if (percentageMana >= 0.6F)
        {
            playerMana[4].enabled = false;
            playerMana[3].enabled = false;
        }
        else if (percentageMana >= 0.5F)
        {
            playerMana[4].enabled = false;
            playerMana[3].enabled = false;
            playerMana[2].sprite = manaHalf;
        }
        else if (percentageMana >= 0.4F)
        {
            playerMana[4].enabled = false;
            playerMana[3].enabled = false;
            playerMana[2].enabled = false;
        }
        else if (percentageMana >= 0.3F)
        {
            playerMana[4].enabled = false;
            playerMana[3].enabled = false;
            playerMana[2].enabled = false;
            playerMana[1].sprite = manaHalf;
        }
        else if (percentageMana >= 0.2F)
        {
            playerMana[4].enabled = false;
            playerMana[3].enabled = false;
            playerMana[2].enabled = false;
            playerMana[1].enabled = false;
        }
        else if (percentageMana >= 0.1F)
        {
            playerMana[4].enabled = false;
            playerMana[3].enabled = false;
            playerMana[2].enabled = false;
            playerMana[1].enabled = false;
            playerMana[0].sprite = manaHalf;
        }
        else if (percentageMana >= 0.0F)
        {
            playerMana[4].enabled = false;
            playerMana[3].enabled = false;
            playerMana[2].enabled = false;
            playerMana[1].enabled = false;
            playerMana[0].enabled = false;
        }
    }

    void nextArrow(int index)
    {
        if (index == 0)
        {
            if (gameController.idArrowEquipment == 0)
            {
                gameController.idArrowEquipment = gameController.arrowsIcons.Length -1;
            }
            else
            {
                gameController.idArrowEquipment -= 1;
            }
        }
        else
        {
            if (gameController.idArrowEquipment == gameController.arrowsIcons.Length -1)
            {
                gameController.idArrowEquipment = 0;
            }
            else
            {
                gameController.idArrowEquipment +=1;
            }
        }
    }
}
