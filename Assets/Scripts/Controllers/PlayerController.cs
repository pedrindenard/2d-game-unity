using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Animator playerAnimator;
    private Rigidbody2D playerRigidBody;

    public Collider2D playerStandardsCollider; // Player standards and crouch collider
    public Collider2D playerCrouchCollider; // Player crouch collide

    public Transform playerInteraction; // Player interaction
    public Transform groundCheck; // Object responsible for detecting if the character is on a surface

    public LayerMask groundLayerMask; // Layer mask for the character that is colliding with the ground
    public LayerMask interactionLayerMask; // Layer mask for the character that is colliding with the interactive object

    public float playerSpeed; // Character movement speed
    public float playerJumpForce; // Force applied to generate the character's jump

    public Vector3 playerDirection = Vector3.right; // Indicates which direction the character is facing
    public PlayerLookingState playerLookingState; // Indicates which direction the character is looking
    public GameObject playerObjectInteraction; // Indicates which object the character is interacting
    public GameObject[] playerWeapons; // Array of weapons

    public bool playerAttacking; // Indicates whether the character is performing an attack
    public bool playerInGround; // Indicates whether the character is stepping on any surface

    private float horizontal;
    private float vertical;

    public int idAnimation; // Indicates which animation is running

    // Initial call from script
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerRigidBody = GetComponent<Rigidbody2D>();
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

    // Called every time the player collides with another collider
    void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Trunk":

                break;
            case "Thorns":

                break;
            case "Plate":

                break;
            case "Lever":

                break;
            case "Door":

                break;
            case "Steps":

                break;
        }
    }

    // Called every time the player left from collides
    void OnCollisionExit2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Trunk":

                break;
            case "Thorns":

                break;
            case "Plate":

                break;
            case "Lever":

                break;
            case "Door":

                break;
            case "Steps":

                break;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {

    }

    void OnTriggerExit2D(Collider2D collider)
    {

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
            playerWeapons[2].SetActive(false);
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
        }
        else
        {
            playerObjectInteraction = null;
        }
    }

    void weaponVisible(bool visible)
    {
        foreach(GameObject gameObject in playerWeapons)
        {
            gameObject.SetActive(visible);
        }
    }

    void weaponControls(int id)
    {
        weaponVisible(false);
        playerWeapons[id].SetActive(true);
    }
}