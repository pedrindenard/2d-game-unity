using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowImageController : MonoBehaviour
{
    
    private GameController gameController;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        gameController = FindObjectOfType(typeof(GameController)) as GameController;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        spriteRenderer.sprite = gameController.arrowsImages[gameController.idArrowEquipment];
    }
}
