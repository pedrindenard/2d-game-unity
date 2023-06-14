using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinIA : MonoBehaviour
{
   
    [Header("CONTROLLERS")]
    public PlayerController playerController;
    public GameController gameController;

    private Rigidbody2D body2D;
    private Animator animator;

    [Header("DIRECTION")]
    public Vector3 enemyDirection = Vector3.right;

    [Header("STATES")]
    public EnemyLookingState enemyLookingState;
    public EnemyStates currentEnemyState;

    [Header("INTERACTIONS")]
    public LayerMask layerPersons;
    public LayerMask layerObstacles;

    [Header("ALERTS")]
    public GameObject alertBalloon;

    [Header("ACTIONS")]
    public bool enemyAttacking; // Indicates whether the character is performing an attack

    [Header("WEAPONS")]
    public GameObject[] enemyWeapons; // Array of weapons
    public GameObject[] enemyBows; // Array of bows
    public GameObject[] enemyStaffs; // Array of staffs
    public GameObject[] enemyArrows; // Array of arrows

    [Header("DISTANCE SETTINGS")]
    public float idleTime;
    public float patrolLimit; // Obstacle limits for collider
    public float playerDistance; // Player vision limits for alert
    public float attackDistance; // Attack distance for player
    public float idleDistance; // Alert off distance

    [Header("VELOCITY")]
    public float baseVelocity;
    public float velocity;

    void Start()
    {
        body2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        gameController = FindObjectOfType(typeof(GameController)) as GameController;
        playerController = FindObjectOfType(typeof(PlayerController)) as PlayerController;

        if (enemyLookingState == EnemyLookingState.LEFT)
        {
            flipEnemy();
        }
    }

    void Update()
    {
        updateInteractivePatrolCollider();
        updateInteractivePlayerVision();
        updateEnemyAttacking();
        updateEnemyMovement();
        updateAlertBalloon();
    }

    void updateInteractivePatrolCollider()
    {
        if (currentEnemyState == EnemyStates.PATROL)
        {
            Debug.DrawRay(transform.position, enemyDirection * patrolLimit, Color.red);
            RaycastHit2D collided = Physics2D.Raycast(transform.position, enemyDirection, patrolLimit, layerObstacles);

            if (collided)
            {
                enemyStates(EnemyStates.STOPPED);
            }
        }
    }

    void updateInteractivePlayerVision()
    {
        Debug.DrawRay(transform.position, enemyDirection * playerDistance, Color.blue);
        RaycastHit2D collided = Physics2D.Raycast(transform.position, enemyDirection, playerDistance, layerPersons);

        if (collided)
        {
            enemyStates(EnemyStates.ALERT);
        }
    }

    void updateEnemyMovement()
    {
        body2D.velocity = new Vector2(velocity, body2D.velocity.y);

        if (velocity == 0)
        {
            animator.SetInteger("IdAnimation", 0);
        }
        else if (velocity != 0)
        {
            animator.SetInteger("IdAnimation", 1);
        }
    }

    void updateEnemyAttacking()
    {
        if (currentEnemyState == EnemyStates.ALERT)
        {
            float playerDistance = Vector3.Distance(transform.position, playerController.transform.position);
            if (playerDistance <= attackDistance)
            {
                enemyStates(EnemyStates.ATTACKING);
            }
            else if (playerDistance >= idleDistance)
            {
                enemyStates(EnemyStates.STOPPED);
            }
        }
    }

    void updateAlertBalloon()
    {
        if (currentEnemyState != EnemyStates.ALERT)
        {
            alertBalloon.SetActive(false);
        }
    }

    void flipEnemy()
    {
        switch (enemyLookingState)
        {
            case EnemyLookingState.LEFT:
                enemyLookingState = EnemyLookingState.RIGHT;
                break;
                
            case EnemyLookingState.RIGHT:
                enemyLookingState = EnemyLookingState.LEFT;
                break;

            default:
                enemyLookingState = EnemyLookingState.LEFT;
                break;
        }

        float scaleX = transform.localScale.x;
        scaleX *= -1; // Invert "scaleX" value

        Vector3 scaleSettings = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);

        transform.localScale = scaleSettings; // Set enemy looking direction
        enemyDirection.x = scaleX; // Set enemy Raycast direction

        baseVelocity *= -1; // Invert enemy velocity
    }

    void enemyStates(EnemyStates state)
    {
        currentEnemyState = state;

        switch (state)
        {
            case EnemyStates.STOPPED:

                velocity = 0; // Stop enemy movement

                StartCoroutine(idle()); // Enemy movement stopped

                break;

            case EnemyStates.ALERT:

                velocity = 0; // Stop enemy movement

                alertBalloon.SetActive(true); // Show alert balloon

                break;
            
            case EnemyStates.PATROL:

                velocity = baseVelocity; // Start enemy movement

                break;
            
            case EnemyStates.ATTACKING:

                velocity = 0; // Stop enemy movement

                animator.SetTrigger("Attack"); // Attack player

                break;
        }
    }

    // Using player animator, so func name need to be playerAttackStates
    void playerAttackStates(int isAttacking)
    {
        // For some reason Unity doesn't understand boolean values in animations functions, so we need to check int values instead
        enemyAttacking = isAttacking != 0;

        if (!enemyAttacking)
        {
            switch (gameController.weaponClasses[gameController.idWeaponPersonCurrent])
            {
                case 0: // Combat
                    enemyWeapons[2].SetActive(false);
                    break;
                
                case 1: // Bow
                    enemyBows[2].SetActive(false);
                    break;
                
                case 2: // Staff
                    enemyStaffs[3].SetActive(false);
                    break;
            }
        }
    }

    // Using player animator, so func name need to be weaponControls
    void weaponControls(int id)
    {
        switch (gameController.weaponClasses[gameController.idWeaponPersonCurrent])
        {
            case 0: // Combat
                weaponVisible(false);
                enemyWeapons[id].SetActive(true);
                break;
            
            case 1: // Bow
                weaponVisible(false);
                enemyBows[id].SetActive(true);
                break;
            
            case 2: // Staff
                weaponVisible(false);
                enemyStaffs[id].SetActive(true);
                break;
        }
    }

    void weaponVisible(bool visible)
    {
        changeVisibility(enemyWeapons, visible);
        changeVisibility(enemyBows, visible);
        changeVisibility(enemyStaffs, visible);
    }

    void changeVisibility(GameObject[] weapons, bool visible)
    {
        foreach(GameObject gameObject in weapons)
        {
            gameObject.SetActive(visible);
        }
    }

    IEnumerator idle()
    {
        yield return new WaitForSeconds(idleTime);
        flipEnemy(); // Change enemy movement direction

        enemyStates(EnemyStates.PATROL);
    }
}