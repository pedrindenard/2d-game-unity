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
    public AmbientLightStates ambientLightStates; // Indicates which ambient light player is currently
    public Vector3 playerDirection = Vector3.right; // Indicates which direction the character is facing
    public PlayerLookingState playerLookingState; // Indicates which direction the character is looking
    public GameObject playerObjectInteraction; // Indicates which object the character is interacting

    [Header("WEAPONS")]
    public GameObject[] playerWeapons; // Array of weapons
    public GameObject[] playerBows; // Array of bows
    public GameObject[] playerStaffs; // Array of staffs
    public GameObject[] playerArrows; // Array of arrows

    [Header("PROJECTILES")]
    public GameObject magic;
    public Transform spawnArrow;
    public Transform spawnMagic;

    [Header("ACTIONS")]
    public bool playerAttacking; // Indicates whether the character is performing an attack
    public bool playerInGround; // Indicates whether the character is stepping on any surface
    public bool playerRecovery; // Indicates when player is recovering from last attack

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

        materialAmbient(ambientLightStates);

        weaponSelected(gameController.idWeaponPerson);
        weaponVisible(false);
    }

    // Called every 0.02 seconds (Player physics simulation)
    void FixedUpdate()
    {
        if (gameController.currentStates != GameStates.PLAY) return; // If game are in pause, return function
        
        playerVelocity();
        playerPhysics();
        playerInteract();
    }

    // Called every frame
    void Update()
    {
        if (gameController.currentStates != GameStates.PLAY) return; // If game are in pause, return function

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        playerLookingStates();
        playerMovingStates();

        attackAnimation();
        jumpAnimation();

        playerCollider();
        playerAnimations();

        interactionAnimation();
        enableDisableArrowAnimation();
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

            case "Enemy":
                decreasePlayerHealth(); // Decrease player health
                break;

            case "Dead":
                gameOver();
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
        if (Input.GetButtonDown("Fire1") && vertical >= 0 && !playerAttacking && playerObjectInteraction == null && !playerRecovery)
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
            
            StartCoroutine(recoveryTimeNextAttack()); // Start player recovery timer
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
        if (gameController.arrowsQuantity[gameController.idArrowEquipment] <= 0) return; // If actual arrow is less than 1, return immediately
        gameController.arrowsQuantity[gameController.idArrowEquipment] -= 1; // Reduce player arrows

        GameObject arrowObj = Instantiate(gameController.arrowsPrefab[gameController.idArrowEquipment], spawnArrow.position, spawnArrow.localRotation);
        changeTransform(arrowObj, gameController.arrowsVelocity[gameController.idArrowEquipment]);
    }

    // Called inside animation "Attack Staff"
    void spawnMagicObject()
    {
        if (gameController.playerCurrentMana <= 0) return; // If actual mana is less than necessary to spawn magic, return immediately
        gameController.playerCurrentMana -= 1; // Reduce player mana

        GameObject magicObj = Instantiate(magic, spawnMagic.position, spawnMagic.localRotation);
        changeTransform(magicObj, 2);
    }

    void changeTransform(GameObject gameObject, int velocity)
    {
        Vector3 magicTransform = gameObject.transform.localScale;

        if (playerLookingState == PlayerLookingState.LEFT)
        {
            gameObject.transform.localScale = new Vector3(magicTransform.x * -1, magicTransform.y, magicTransform.z);
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(velocity * -1, 0);
        }
        else
        {
            gameObject.transform.localScale = new Vector3(magicTransform.x, magicTransform.y, magicTransform.z);
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(velocity, 0);
        }

        Destroy(gameObject, 1); // Destroy magic after 1s
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

    void enableDisableArrowAnimation()
    {
        foreach (GameObject arrow in playerArrows)
        {
            arrow.SetActive(gameController.arrowsQuantity[gameController.idArrowEquipment] > 0); // Active arrow animation
        }
    }

    void decreasePlayerHealth()
    {
        if (gameController.playerCurrentHealth <= 0) return; // Player is dead
        
        gameController.playerCurrentHealth -= 3; // Remove percentage player health
        
        if (gameController.playerCurrentHealth < 0)
        {
            gameOver();
        }
    }

    void gameOver()
    {
        gameController.playerCurrentHealth = 0; // Set player health to 0 if less than 0
        gameController.currentStates = GameStates.DEAD; // Game over
        StartCoroutine(startDeadAnimation()); // Start dead animation
    }

    void materialAmbient(AmbientLightStates ambient)
    {
        switch (ambient)
        {
            case AmbientLightStates.LIGHT:
                setPlayerMaterial2D(gameController.defaultMaterial);
                break;

            case AmbientLightStates.NIGHT:
                setPlayerMaterial2D(gameController.lightMaterial);
                break;
        }
    }

    IEnumerator recoveryTimeNextAttack()
    {
        playerRecovery = true; // Start player recovery
        yield return new WaitForSeconds(0.2F);
        playerRecovery = false; // End player recovery after a certain amount of time
    }

    IEnumerator startDeadAnimation()
    {
        playerAnimator.SetInteger("IdAnimation", 3);
        yield return new WaitForSeconds(1F);

        gameController.menuDead(); // Show game over menu
        gameController.gameStates(GameStates.DEAD); // Set game states to game over

        gameObject.GetComponent<Animator>().enabled = false; // Disable animation
    }
}