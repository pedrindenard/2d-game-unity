using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SkinController : MonoBehaviour
{

    public CharacterStates characterStates;

    private GameController gameController;
    private SpriteRenderer spriteRenderer;
    private Sprite[] sprites;

    public string spriteSheetName;
    private string spriteSheetLoadedName;

    private Dictionary<string, Sprite> spriteSheet;

    void Start()
    {
        loadSpriteRenderer(); // Get sprite renderer component
        loadSpritesFromResources(); // Load new sprite sheet
        loadGameController(); // Load current controller
        loadVariables();
    }
    
    void LateUpdate()
    {
        if (spriteSheetLoadedName != spriteSheetName) // If current sprit is != from selected sprite, change it
        {
            loadSpritesFromResources(); // Load new sprite sheet
        }

        if (gameController.idPerson != gameController.idPersonCurrent)
        {
            loadVariables();
        }

        if (characterStates == CharacterStates.PLAYER)
        {
            gameController.validateWeapon();
        }

        spriteRenderer.sprite = spriteSheet[spriteRenderer.sprite.name]; // Get sprite by sprite name and change it to actual
    }

    void loadSpritesFromResources()
    {
        sprites = Resources.LoadAll<Sprite>(spriteSheetName); // Load all sprites from Resources
        spriteSheet = sprites.ToDictionary(x => x.name, x => x); // Transform sprites into Dictionary which current name
        spriteSheetLoadedName = spriteSheetName; // Set actual name of sprite selected
    }

    void loadSpriteRenderer()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void loadGameController()
    {
        gameController = FindObjectOfType(typeof(GameController)) as GameController;
    }

    void loadVariables()
    {
        if (characterStates == CharacterStates.PLAYER)
        {
            spriteSheetName = gameController.personSprites[gameController.idPerson].name;
            gameController.updatePlayerSkin();
        }
    }
}
