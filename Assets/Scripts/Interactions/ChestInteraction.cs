using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInteraction : MonoBehaviour
{

    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    public Collider2D chestCollider;

    [Header("Renders")]
    public Sprite[] spriteAnimations;

    public GameObject[] loots;
    private bool opened;

    void Start()
    {
        findComponentRender();
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
        spriteRenderer.sprite = spriteAnimations[1]; // Change sprite
        chestCollider.enabled = false; // Disable collider
        opened = true; // Change to opened state
    }

    void findComponentRender() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        chestCollider = GetComponent<Collider2D>();
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
