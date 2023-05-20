using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInteraction : MonoBehaviour
{

    private GameController gameController;

    private SpriteRenderer spriteRenderer;
    public Sprite[] spriteAnimations;

    private bool opened;

    void Start()
    {
        findGameController();
        findSpriteRender();
    }

    void interaction()
    {
        openCloseChest();
    }

    void openCloseChest()
    {
        opened = !opened;

        switch (opened)
        {
            case true:
                spriteRenderer.sprite = spriteAnimations[1];
                break;
                
            case false:
                spriteRenderer.sprite = spriteAnimations[0];
                break;
        }   
    }

    void openChest()
    {
        if (opened) return;
        opened = true;
        spriteRenderer.sprite = spriteAnimations[1];
    }

    void findGameController()
    {
        if (gameController == null) gameController = FindObjectOfType(typeof(GameController)) as GameController;
    }

    void findSpriteRender() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
