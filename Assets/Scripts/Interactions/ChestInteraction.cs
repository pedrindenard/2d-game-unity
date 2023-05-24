using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInteraction : MonoBehaviour
{

    [Header("Controllers")]
    private GameController gameController;

    [Header("Renders")]
    private SpriteRenderer spriteRenderer;
    public Sprite[] spriteAnimations;

    public GameObject[] loots;
    private bool opened;

    void Start()
    {
        findGameController();
        findSpriteRender();
    }

    void interaction()
    {
        openChest();
    }

    void openChest()
    {
        if (opened) return;
        chestAnimation();
        showLoots();
    }

    void chestAnimation()
    {
        spriteRenderer.sprite = spriteAnimations[1];
        opened = true;
    }

    void findGameController()
    {
        gameController = FindObjectOfType(typeof(GameController)) as GameController;
    }

    void findSpriteRender() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void showLoots()
    {
        foreach (var item in loots) // Scroll through the loot lis
        {
            GameObject loot = Instantiate(item, transform.position, transform.localRotation); // Show loots
            Vector2 lootForce = new Vector2(Random.Range(-25, 25), Random.Range(75, 100)); // Loot force effects

            loot.GetComponent<Rigidbody2D>().AddForce(lootForce); // Add force in loot
        }
    }
}
