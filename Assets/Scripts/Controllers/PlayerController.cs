using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Animator playerAnimator;
    private Rigidbody2D playerRigidBody;
    private GameController gameController;
    private SpriteRenderer playerRenderer;

    [Header("COLLIDERS")]
    public Collider2D playerStandardsCollider; // Player standards and crouch collider
    public Collider2D playerCrouchCollider; // Player crouch collide

    [Header("INTERACTIONS")]
    public GameObject playerAlertInteraction; // Show interaction animation
    public Transform playerInteraction; // Player interaction
    public Transform groundCheck; // Object responsible for detecting if the character is on a surface

    [Header("MASKS")]
    public LayerMask groundLayerMask; // Layer mask for the character that is colliding with the ground
    public LayerMask interactionLayerMask; // Layer mask for the character that is colliding with the interactive object

    [Header("FORCES")]
    public float playerSpeed; // Character movement speed
    public float playerJumpForce; // Force applied to generate the character's jump

    [Header("STATES")]
    public Vector3 playerDirection = Vector3.right; // Indicates which direction the character is facing
    public PlayerLookingState playerLookingState; // Indicates which direction the character is looking
    public GameObject playerObjectInteraction; // Indicates which object the character is interacting

    [Header("WEAPONS")]
    public GameObject[] playerWeapons; // Array of weapons
    public GameObject[] playerBows; // Array of bows
    public GameObject[] playerStaffs; // Array of staffs
    public GameObject[] playerArrows; // Array of arrows

    [Header("PROJECTILES")]
    public GameObject arrow;
    public GameObject magic;
    public Transform spawnArrow;
    public Transform spawnMagic;

    [Header("ACTIONS")]
    public bool playerAttacking; // Indicates whether the character is performing an attack
    public bool playerInGround; // Indicates whether the character is stepping on any surface

    [Header("HEALTH")]
    public int playerMaxHealth; // Max health player can have and current health
    public int playerCurrentHealth; // Current player health

    private float horizontal;
    private float vertical;

    public int idAnimation; // Indicates which animation is running

    // Initial call from script
    void Start()
    {   
        gameController = FindObjectOfType(typeof(GameController)) as GameController;

        playerRenderer = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
        playerRigidBody = GetComponent<Rigidbody2D>();

        playerMaxHealth = gameController.maxLifePerson;

        playerCurrentHealth = playerMaxHealth;
                
        weaponSelected(gameController.idWeaponPerson);
        weaponVisible(false);
    }

    // Called every 0.02 seconds (Player physics simulation)
    void FixedUpdate()
    {
        playerVelocity();
        playerPhysics();
        playerInteract();
    }

    // Called every frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        playerLookingStates();
        playerMovingStates();

        attackAnimation();
        jumpAnimation();

        playerCollider();
        playerAnimations();

        interactionAnimation();
    }

    void LateUpdate()
    {
        if (gameController.idWeaponPerson != gameController.idWeaponPersonCurrent)
        {
            weaponSelected(gameController.idWeaponPerson);
        }
    }

    // Called every time the player collides with trigger
    void OnTriggerEnter2D(Collider2D collider)
    {
        switch (collider.gameObject.tag)
        {
            case "Loots":
                collider.gameObject.SendMessage("gather", SendMessageOptions.DontRequireReceiver);
                break;
        }
    }

    void playerMovingStates()
    {
        if (vertical < 0)
        {
            // Player is crouching
            idAnimation = 2;
            // If player is crouching, cannot move. But if player is grounded, can move
            if (groundCheck) horizontal = 0;
        }
        else if (horizontal != 0)
        {
            // Player is walking
            idAnimation = 1;
        }
        else
        {
            // Player is idle
            idAnimation = 0;
        }
    }

    void playerLookingStates()
    {
        if (horizontal > 0 && playerLookingState == PlayerLookingState.LEFT && !playerAttacking)
        {
            flipPlayer();
        }
        else if (horizontal < 0 && playerLookingState == PlayerLookingState.RIGHT && !playerAttacking)
        {
            flipPlayer();
        }
    }

    void playerCollider()
    {
        playerStandardsCollider.enabled = (vertical >= 0 && playerInGround) || ((vertical != 0 || vertical >= 0) && !playerInGround);
        playerCrouchCollider.enabled = vertical < 0 && playerInGround;
    }

    void playerAnimations()
    {
        playerAnimator.SetBool("Grounded", playerInGround);
        playerAnimator.SetInteger("IdAnimation", idAnimation);
        playerAnimator.SetFloat("SpeedY", playerRigidBody.velocity.y);
        playerAnimator.SetFloat("IdWeaponClass", gameController.weaponClasses[gameController.idWeaponPersonCurrent]);
    }

    void attackAnimation()
    {
        if (Input.GetButtonDown("Fire1") && vertical >= 0 && !playerAttacking && playerObjectInteraction == null)
        {
            playerAnimator.SetTrigger("Attack");
        }
        // If player is running attack animation, stop moving
        if (playerAttacking && playerInGround)
        {
            horizontal = 0;
        }
    }

    void interactionAnimation()
    {
        if (Input.GetButtonDown("Fire1") && vertical >= 0 && !playerAttacking && playerObjectInteraction != null)
        {
            playerObjectInteraction.SendMessage("interaction", SendMessageOptions.DontRequireReceiver);
        }
    }

    void jumpAnimation()
    {
        if (Input.GetButtonDown("Jump") && playerInGround && !playerAttacking)
        {
            Vector2 jumpVector = new Vector2(0, playerJumpForce);
            playerRigidBody.AddForce(jumpVector);
        }
    }

    void flipPlayer()
    {
        switch (playerLookingState)
        {
            case PlayerLookingState.LEFT:
                playerLookingState = PlayerLookingState.RIGHT;
                break;
                
            case PlayerLookingState.RIGHT:
                playerLookingState = PlayerLookingState.LEFT;
                break;

            default:
                playerLookingState = PlayerLookingState.LEFT;
                break;
        }

        float scaleX = transform.localScale.x;
        scaleX *= -1; // Invert "scaleX" value

        Vector3 scaleSettings = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);

        transform.localScale = scaleSettings; // Set player looking direction
        playerDirection.x = scaleX; // Set player Raycast direction
    }

    public void playerAttackStates(int isAttacking)
    {
        // For some reason Unity doesn't understand boolean values in animations functions, so we need to check int values instead
        playerAttacking = isAttacking != 0;

        if (!playerAttacking)
        {
            switch (gameController.weaponClasses[gameController.idWeaponPersonCurrent])
            {
                case 0: // Combat
                    playerWeapons[2].SetActive(false);
                    break;
                
                case 1: // Bow
                    playerBows[2].SetActive(false);
                    break;
                
                case 2: // Staff
                    playerStaffs[3].SetActive(false);
                    break;
            }
        }
    }

    void playerVelocity()
    {
        playerRigidBody.velocity = new Vector2(horizontal * playerSpeed, playerRigidBody.velocity.y);
    }

    void playerPhysics()
    {
        playerInGround = Physics2D.OverlapCircle(groundCheck.position, 0.02F, groundLayerMask);
    }

    void playerInteract()
    {
        // Create invisible line ray for player interactions
        RaycastHit2D hit = Physics2D.Raycast(playerInteraction.position, playerDirection, 0.15F, interactionLayerMask);

        // Show line in scene mode
        Debug.DrawRay(playerInteraction.position, playerDirection * 0.15F, Color.red);

        if (hit)
        {
            playerObjectInteraction = hit.collider.gameObject;
            playerAlertInteraction.SetActive(true);
        }
        else
        {
            playerObjectInteraction = null;
            playerAlertInteraction.SetActive(false);
        }
    }

    void weaponVisible(bool visible)
    {
        changeVisibility(playerWeapons, visible);
        changeVisibility(playerBows, visible);
        changeVisibility(playerStaffs, visible);
    }

    void weaponControls(int id)
    {
        switch (gameController.weaponClasses[gameController.idWeaponPersonCurrent])
        {
            case 0: // Combat
                weaponVisible(false);
                playerWeapons[id].SetActive(true);
                break;
            
            case 1: // Bow
                weaponVisible(false);
                playerBows[id].SetActive(true);
                break;
            
            case 2: // Staff
                weaponVisible(false);
                playerStaffs[id].SetActive(true);
                break;
        }
    }

    public void weaponSelected(int id)
    {
        gameController.idWeaponPerson = id; // Set weapon identifier

        switch (gameController.weaponClasses[id])
        {
            case 0: // Combat
                combatWeapon(id);
                break;

            case 1: // Bow
                bowWeapon(id);
                break;

            case 2: // Staff
                staffWeapon(id);
                break;
        }

        gameController.idWeaponPersonCurrent = gameController.idWeaponPerson; // Set current weapon
    }

    void combatWeapon(int id)
    {
        playerWeapons[0].GetComponent<SpriteRenderer>().sprite = gameController.weaponStage(WeaponStageStates.ONE, id);
        playerWeapons[1].GetComponent<SpriteRenderer>().sprite = gameController.weaponStage(WeaponStageStates.TWO, id);
        playerWeapons[2].GetComponent<SpriteRenderer>().sprite = gameController.weaponStage(WeaponStageStates.THREE, id);

        WeaponInformation weaponStage1 = playerWeapons[0].GetComponent<WeaponInformation>();
        WeaponInformation weaponStage2 = playerWeapons[1].GetComponent<WeaponInformation>();
        WeaponInformation weaponStage3 = playerWeapons[2].GetComponent<WeaponInformation>();

        int newMinDamage = gameController.minDamage[id];
        int newMaxDamage = gameController.maxDamage[id];
        int newEffectDamage = gameController.damageEffect[id];

        weaponStage1.minWeaponDamage = newMinDamage;
        weaponStage1.maxWeaponDamage = newMaxDamage;
        weaponStage1.weaponDamageType = newEffectDamage;

        weaponStage2.minWeaponDamage = newMinDamage;
        weaponStage2.maxWeaponDamage = newMaxDamage;
        weaponStage2.weaponDamageType = newEffectDamage;

        weaponStage3.minWeaponDamage = newMinDamage;
        weaponStage3.maxWeaponDamage = newMaxDamage;
        weaponStage3.weaponDamageType = newEffectDamage;
    }

    void bowWeapon(int id)
    {
        playerBows[0].GetComponent<SpriteRenderer>().sprite = gameController.weaponStage(WeaponStageStates.ONE, id);
        playerBows[1].GetComponent<SpriteRenderer>().sprite = gameController.weaponStage(WeaponStageStates.TWO, id);
        playerBows[2].GetComponent<SpriteRenderer>().sprite = gameController.weaponStage(WeaponStageStates.THREE, id);
    }

    void staffWeapon(int id)
    {
        playerStaffs[0].GetComponent<SpriteRenderer>().sprite = gameController.weaponStage(WeaponStageStates.ONE, id);
        playerStaffs[1].GetComponent<SpriteRenderer>().sprite = gameController.weaponStage(WeaponStageStates.TWO, id);
        playerStaffs[2].GetComponent<SpriteRenderer>().sprite = gameController.weaponStage(WeaponStageStates.THREE, id);
        playerStaffs[3].GetComponent<SpriteRenderer>().sprite = gameController.weaponStage(WeaponStageStates.FOUR, id);
    }

    public void setPlayerMaterial2D(Material material)
    {
        playerRenderer.material = material;

        changeMaterial(playerWeapons, material);
        changeMaterial(playerBows, material);
        changeMaterial(playerArrows, material);
        changeMaterial(playerStaffs, material);
    }

    // Called inside animation "Attack Bow"
    void spawnArrowObject()
    {
        GameObject arrowObj = Instantiate(arrow, spawnArrow.position, spawnArrow.localRotation);
        Vector3 arrowTransform = arrowObj.transform.localScale;

        arrowObj.transform.localScale = new Vector3(arrowTransform.x, arrowTransform.y, arrowTransform.z);
        arrowObj.GetComponent<Rigidbody2D>().velocity = new Vector3(5 * playerDirection.x, 0);

        Destroy(arrowObj, 1); // Destroy magic after 1s
    }

    // Called inside animation "Attack Staff"
    void spawnMagicObject()
    {
        GameObject magicObj = Instantiate(magic, spawnMagic.position, spawnMagic.localRotation);
        Vector3 magicTransform = magicObj.transform.localScale;

        magicObj.transform.localScale = new Vector3(magicTransform.x, magicTransform.y, magicTransform.z);
        magicObj.GetComponent<Rigidbody2D>().velocity = new Vector3(5 * playerDirection.x, 0);

        Destroy(magicObj, 1); // Destroy magic after 1s
    }

    void changeMaterial(GameObject[] weapons, Material material)
    {
        foreach (GameObject weapon in weapons)
        {
            weapon.GetComponent<SpriteRenderer>().material = material;
        }
    }

    void changeVisibility(GameObject[] weapons, bool visible)
    {
        foreach(GameObject gameObject in weapons)
        {
            gameObject.SetActive(visible);
        }
    }
}