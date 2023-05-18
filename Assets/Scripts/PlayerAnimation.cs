using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
    
    private Animator playerAnimator;
    private Rigidbody2D playerRigidBody;

    public Transform groundCheck;

    public float playerSpeed;
    public float playerJumpForce;

    public PlayerLookingState playerLookingState;
    public bool playerInGround;

    private float horizontal;
    private float vertical;

    public int idAnimation;

    // Initial call from script
    void Start() {
        playerAnimator = GetComponent<Animator>();
        playerRigidBody = GetComponent<Rigidbody2D>();
    }

    // Called every 0.02 seconds (Player physics simulation)
    void FixedUpdate() {
        playerRigidBody.velocity = new Vector2(horizontal * playerSpeed, playerRigidBody.velocity.y);
        playerInGround = Physics2D.OverlapCircle(groundCheck.position, 0.02F);
    }

    // Called every frame
    void Update() {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        playerLookingStates();
        playerMovingStates();

        attackAnimation();
        jumpAnimation();

        playerAnimations();
    }

    void playerMovingStates() {
        if (vertical < 0) {
            // Player is crouching
            idAnimation = 2;
            // If player is crouching, cannot move
            horizontal = 0;
        } else if (horizontal != 0) {
            idAnimation = 1;
        } else {
            idAnimation = 0;
        }
    }

    void playerLookingStates() {
        if (horizontal > 0 && playerLookingState == PlayerLookingState.LEFT) {
            flipPlayer();
        } else if (horizontal < 0 && playerLookingState == PlayerLookingState.RIGHT) {
            flipPlayer();
        }
    }

    void playerAnimations() {
        playerAnimator.SetBool("Grounded", playerInGround);
        playerAnimator.SetInteger("IdAnimation", idAnimation);
        playerAnimator.SetFloat("SpeedY", playerRigidBody.velocity.y);
    }

    void attackAnimation() {
        if (Input.GetButtonDown("Fire1") && vertical >= 0) {
            playerAnimator.SetTrigger("Attack");
        }
    }

    void jumpAnimation() {
        if (Input.GetButtonDown("Jump") && playerInGround) {
            Vector2 jumpVector = new Vector2(0, playerJumpForce);
            playerRigidBody.AddForce(jumpVector);
        }
    }

     void flipPlayer() {
        switch (playerLookingState) {
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
        scaleX *= -1; // Inverter o valor do "scaleX"

        Vector3 scaleSettings = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
        transform.localScale = scaleSettings;
    }

}