using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIAController : MonoBehaviour
{
   
    [Header("CONTROLLERS")]
    public PlayerController playerController;
    public GameController gameController;

    [Header("PARAMETERS")]
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D body2D;
    private Animator animator;

    [Header("DIRECTION")]
    public Vector3 enemyDirection = Vector3.right;

    [Header("STATES")]
    public AmbientLightStates ambientLightStates;
    public EnemyLookingState enemyLookingState;
    public EnemyStates currentEnemyState;

    [Header("INTERACTIONS")]
    public LayerMask layerPersons;
    public LayerMask layerObstacles;

    [Header("ALERTS")]
    public GameObject alertBalloon;

    [Header("ACTIONS")]
    public bool enemyAttacking; // Indicates whether the character is performing an attack
    public bool enemyAlert; // Indicates whether the character is in alert

    [Header("WEAPONS")]
    public int weaponId; // Enemy weapon identifier
    public int weaponClassId; // Weapon class identifier
    public GameObject[] enemyWeapons; // Array of weapons
    public GameObject[] enemyBows; // Array of bows
    public GameObject[] enemyStaffs; // Array of staffs
    public GameObject[] enemyArrows; // Array of arrows

    [Header("PROJECTILES")]
    public GameObject magic;
    public Transform spawnArrow;
    public Transform spawnMagic;

    [Header("DISTANCE SETTINGS")]
    public float idleTime;
    public float fallbackTime;
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
        spriteRenderer = GetComponent<SpriteRenderer>();

        gameController = FindObjectOfType(typeof(GameController)) as GameController;
        playerController = FindObjectOfType(typeof(PlayerController)) as PlayerController;

        if (enemyLookingState == EnemyLookingState.LEFT)
        {
            flipEnemy();
        }

        enemyStates(EnemyStates.PATROL); // Default state

        materialAmbient(ambientLightStates); // Changes sprite rendering to ambient lighting

        weaponSelected(weaponClassId); // Enemy default weapon;
    }

    void Update()
    {
        updateInteractivePatrolColliderWhenFallback();
        updateInteractivePatrolCollider();
        updateInteractivePlayerVision();
        updateEnemyAttacking();
        updateEnemyMovement();
        updateAlertBalloon();

        // Animator updates
        animator.SetFloat("IdWeaponClass", weaponClassId);
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

    void updateInteractivePatrolColliderWhenFallback()
    {
        if (currentEnemyState == EnemyStates.FALLBACK)
        {
            Debug.DrawRay(transform.position, enemyDirection * patrolLimit, Color.red);
            RaycastHit2D collided = Physics2D.Raycast(transform.position, enemyDirection, patrolLimit, layerObstacles);

            if (collided)
            {
                flipEnemy(); // Change enemy movement
            }
        }
    }

    void updateInteractivePlayerVision()
    {
        if (currentEnemyState != EnemyStates.ATTACKING && currentEnemyState != EnemyStates.FALLBACK)
        {
            Debug.DrawRay(transform.position, enemyDirection * playerDistance, Color.blue);
            RaycastHit2D collided = Physics2D.Raycast(transform.position, enemyDirection, playerDistance, layerPersons);

            if (collided)
            {
                enemyStates(EnemyStates.ALERT);
            }
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
            else if (playerDistance >= idleDistance && !enemyAlert)
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

        baseVelocity *= -1; // Invert enemy base velocity

        float velocityActual = velocity * -1; // Invert
        velocity = velocityActual; // Update enemy velocity
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

            case EnemyStates.FALLBACK:

                flipEnemy(); // Flip enemy to fallback

                velocity = baseVelocity * 1.2F; // Increment enemy movement

                StartCoroutine(fallback()); // Enemy movement fallback

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
            switch (weaponClassId)
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

        if (isAttacking == 0)
        {
            enemyStates(EnemyStates.FALLBACK);
        }
    }

    // Using player animator, so func name need to be weaponControls
    void weaponControls(int id)
    {
        switch (weaponClassId)
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

    void weaponSelected(int classId)
    {
        switch (classId)
        {
            case 0: // Combat
                combatWeapon(weaponId);
                break;

            case 1: // Bow
                bowWeapon(weaponId);
                break;

            case 2: // Staff
                staffWeapon(weaponId);
                break;
        }
    }

    void combatWeapon(int id)
    {
        enemyWeapons[0].GetComponent<SpriteRenderer>().sprite = gameController.weaponStage(WeaponStageStates.ONE, id);
        enemyWeapons[1].GetComponent<SpriteRenderer>().sprite = gameController.weaponStage(WeaponStageStates.TWO, id);
        enemyWeapons[2].GetComponent<SpriteRenderer>().sprite = gameController.weaponStage(WeaponStageStates.THREE, id);

        WeaponInformation weaponStage1 = enemyWeapons[0].GetComponent<WeaponInformation>();
        WeaponInformation weaponStage2 = enemyWeapons[1].GetComponent<WeaponInformation>();
        WeaponInformation weaponStage3 = enemyWeapons[2].GetComponent<WeaponInformation>();

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
        enemyBows[0].GetComponent<SpriteRenderer>().sprite = gameController.weaponStage(WeaponStageStates.ONE, id);
        enemyBows[1].GetComponent<SpriteRenderer>().sprite = gameController.weaponStage(WeaponStageStates.TWO, id);
        enemyBows[2].GetComponent<SpriteRenderer>().sprite = gameController.weaponStage(WeaponStageStates.THREE, id);
    }

    void staffWeapon(int id)
    {
        enemyStaffs[0].GetComponent<SpriteRenderer>().sprite = gameController.weaponStage(WeaponStageStates.ONE, id);
        enemyStaffs[1].GetComponent<SpriteRenderer>().sprite = gameController.weaponStage(WeaponStageStates.TWO, id);
        enemyStaffs[2].GetComponent<SpriteRenderer>().sprite = gameController.weaponStage(WeaponStageStates.THREE, id);
        enemyStaffs[3].GetComponent<SpriteRenderer>().sprite = gameController.weaponStage(WeaponStageStates.FOUR, id);
    }

    // Called inside animation "Attack Bow"
    void spawnArrowObject()
    {
        GameObject arrowObj = Instantiate(gameController.arrowsPrefab[gameController.idArrowEquipment], spawnArrow.position, spawnArrow.localRotation);
        changeTransform(arrowObj, gameController.arrowsVelocity[gameController.idArrowEquipment]);
    }

    // Called inside animation "Attack Staff"
    void spawnMagicObject()
    {
        GameObject magicObj = Instantiate(magic, spawnMagic.position, spawnMagic.localRotation);
        changeTransform(magicObj, 2);
    }

    void changeTransform(GameObject gameObject, int velocity)
    {
        Vector3 magicTransform = gameObject.transform.localScale;

        if (enemyLookingState == EnemyLookingState.LEFT)
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

    public void onDamageReceived()
    {
        enemyAlert = true; // Set enemy alert for next seconds

        enemyStates(EnemyStates.ALERT); // If receive any damage, stay alert

        StartCoroutine(damageReceiveAlert()); // Stay in alert for 3 seconds
    }

    void materialAmbient(AmbientLightStates ambient)
    {
        switch (ambient)
        {
            case AmbientLightStates.LIGHT:
                setEnemyMaterial2D(gameController.defaultMaterial);
                break;

            case AmbientLightStates.NIGHT:
                setEnemyMaterial2D(gameController.lightMaterial);
                break;
        }
    }

    void setEnemyMaterial2D(Material material)
    {
        spriteRenderer.material = material;

        changeMaterial(enemyWeapons, material);
        changeMaterial(enemyBows, material);
        changeMaterial(enemyArrows, material);
        changeMaterial(enemyStaffs, material);
    }

    void changeMaterial(GameObject[] weapons, Material material)
    {
        foreach (GameObject weapon in weapons)
        {
            weapon.GetComponent<SpriteRenderer>().material = material;
        }
    }

    IEnumerator idle()
    {
        yield return new WaitForSeconds(idleTime);
        flipEnemy(); // Change enemy movement direction

        enemyStates(EnemyStates.PATROL);
    }

    IEnumerator fallback()
    {
        yield return new WaitForSeconds(fallbackTime);
        flipEnemy(); // Change enemy movement direction

        enemyStates(EnemyStates.ALERT);
    }

    IEnumerator damageReceiveAlert()
    {
        yield return new WaitForSeconds(3); // Wait for 3 seconds
        enemyAlert = false; // Disable alert interaction
    }
}