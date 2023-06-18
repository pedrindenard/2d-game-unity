using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    [Header("Enemy controller")]
    private GameController gameController;
    private PlayerController playerController;
    private EnemyIAController enemyIAController;
    private SpriteRenderer enemySpriteRenderer;
    private Animator enemyAnimator;

    [Header("Enemy ground collider")]
    public Transform groundCheck; // Object responsible for detecting if the character is on a surface
    public LayerMask groundLayerMask; // Layer mask for the character that is colliding with the ground

    [Header("Enemy life")]
    public float enemyLife;
    public float enemyCurrentLife;
    public float enemyPercentageLife;
    public GameObject enemyHPBarConfig;
    public Transform enemyHPBarLife;

    [Header("Enemy damage and color adjustment")]
    public float[] enemyDamageAdjustments;
    public Color[] enemyStatesColor;

    [Header("Enemy states")]
    public EnemyHitState enemyHitState;
    public EnemyLookingState enemyLookingState;
    public PlayerLookingState playerLookingState;

    [Header("Enemy knock force")]
    public GameObject knockForcePrefab; // Repulsive force
    public Transform knockPosition; // Position force\
    private float knockXTemp; // Knock X position temp
    public float knockX; // Knock X position

    [Header("Enemy loots drop")]
    public GameObject[] loots;

    void Start()
    {
        findControllers();
        findEnemyComponents();
        findEnemyInitialStates();
    }

    void Update()
    {
        checkPlayerPosition();

        // Animator updates
        enemyAnimator.SetBool("Grounded", true);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        switch (collider.gameObject.tag)
        {
            case "Weapon":

                if (enemyHitState == EnemyHitState.VULNERABLE)
                {
                    gameObject.SendMessage("onDamageReceived", SendMessageOptions.DontRequireReceiver); // Change IA states

                    enemyHitState = EnemyHitState.IMMUNE; // Set to IMMUNE for block double attack when animation git is running
                    enemyHPBarConfig.SetActive(true); // Show enemy health bar
                    enemyAnimator.SetTrigger("Hit"); // Show enemy hit animation

                    WeaponInformation weapon = collider.gameObject.GetComponent<WeaponInformation>();

                    int weaponDamageType = weapon.weaponDamageType;

                    reduceEnemyLife(weaponDamageType, weapon); // Reduce enemy health by damage attack received by player

                    setEnemyHealthBarPercent(); // Set enemy health bar current life
                    killEnemyIfThereNoMoreLife(); // Kill enemy and destroy object

                    showHitEffect(weaponDamageType); // Show hit effect by weapon type
                    showKnockEffect(); // Show knock effect to pull enemy fall
                    
                    StartCoroutine(enemyImmune()); // Change sprite renderer color to red to indicate that player hit damage
                }

                break;
        }
    }

    void findControllers()
    {
        gameController = FindObjectOfType(typeof(GameController)) as GameController;
        playerController = FindObjectOfType(typeof(PlayerController)) as PlayerController;
        enemyIAController = FindObjectOfType(typeof(EnemyIAController)) as EnemyIAController;
    }

    void findKnockXPosition(float x)
    {
        knockPosition.localPosition = new Vector3(x, knockPosition.localPosition.y, 0);
    }

    void findEnemyComponents()
    {
        enemySpriteRenderer = GetComponent<SpriteRenderer>();
        enemyAnimator = GetComponent<Animator>();
    }

    void findInitialEnemyStateColor()
    {
        enemySpriteRenderer.color = enemyStatesColor[0];
    }

    void findEnemyInitialStates()
    {
        enemySpriteRenderer.color = enemyStatesColor[0]; // Set enemy color to write
        enemyHPBarLife.localScale = new Vector3(1, 1, 1); // Set enemy bar to full HP
        enemyCurrentLife = enemyLife; // Set current enemy life
        enemyHPBarConfig.SetActive(false); // Hidden health bar
    }

    void checkPlayerPosition()
    {
        float xPlayer = playerController.transform.position.x;
        float xEnemy = transform.position.x;

        if (xPlayer < xEnemy)
        {
            playerLookingState = PlayerLookingState.LEFT;
        }
        else if (xPlayer > xEnemy)
        {
            playerLookingState = PlayerLookingState.RIGHT;
        }

        flipEnemyKnock();
    }

    void changeEnemyLookingState()
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
    }

    void flipEnemy()
    {
        float scaleX = transform.localScale.x;
        float scaleXHealthBar = enemyHPBarConfig.transform.localScale.x;
        
        scaleXHealthBar *= -1; // Invert "scaleX" value
        scaleX *= -1; // Invert "scaleX" value

        Vector3 scaleHealthBarSettings = enemyHPBarConfig.transform.localScale;
        Vector3 scaleSettings = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
        Vector3 scaleHealth = new Vector3(scaleXHealthBar, scaleHealthBarSettings.y, scaleHealthBarSettings.z);
        
        transform.localScale = scaleSettings; // Set enemy looking direction
        enemyHPBarConfig.transform.localScale = scaleHealth; // Set enemy health bar direction
    }

    void flipEnemyKnock()
    {
        if (enemyLookingState == EnemyLookingState.LEFT && playerLookingState == PlayerLookingState.LEFT)
        {
            knockXTemp = knockX;
        }
        else if (enemyLookingState == EnemyLookingState.RIGHT && playerLookingState == PlayerLookingState.LEFT)
        {
            knockXTemp = knockX * -1; // Invert knock scale
        }
        else if (enemyLookingState == EnemyLookingState.LEFT && playerLookingState == PlayerLookingState.RIGHT)
        {
            knockXTemp = knockX * -1; // Invert knock scale
        }
        else if (enemyLookingState == EnemyLookingState.RIGHT && playerLookingState == PlayerLookingState.RIGHT)
        {
            knockXTemp = knockX;
        }
        findKnockXPosition(knockXTemp);
    }

    void setEnemyHealthBarPercent()
    {
        enemyPercentageLife = enemyCurrentLife / enemyLife; // Calculate enemy health percentage
        if (enemyPercentageLife < 0) enemyPercentageLife = 0; // Fix enemy's life if less than 0
        enemyHPBarLife.localScale = new Vector3(enemyPercentageLife, 1, 1);
    }

    void killEnemyIfThereNoMoreLife()
    {
        if (enemyCurrentLife <= 0)
        {
            enemyHitState = EnemyHitState.DIE; // Indicate death state
            enemyAnimator.SetInteger("IdAnimation", 3); // Start enemy death animation

            gameObject.layer = LayerMask.NameToLayer("Ignore"); // Set layer to player to not interact with player

            StartCoroutine(enemyLoots()); // Start enemy death and loot animation
        }
    }

    void showHitEffect(int weaponDamageType)
    {
        GameObject hitEffect = gameController.weaponAttackEffects[weaponDamageType];
        GameObject hitEffectTemp = Instantiate(hitEffect, transform.position, transform.localRotation);
        Destroy(hitEffectTemp, 1F); // Destroy hit effects after 1s
    }

    void showKnockEffect()
    {
        GameObject knockTemp = Instantiate(knockForcePrefab, knockPosition.position, knockPosition.localRotation);
        Destroy(knockTemp, 0.03F); // Destroy knock object after 0.03s
    }
    
    void reduceEnemyLife(int weaponDamageType, WeaponInformation weapon)
    {
        float weaponDamage = Random.Range(weapon.minWeaponDamage, weapon.maxWeaponDamage);;
        float weaponBonusDamage = weaponDamage + (weaponDamage * (enemyDamageAdjustments[weaponDamageType] / 100));

        enemyCurrentLife -= Mathf.RoundToInt(weaponBonusDamage); // Reduce enemy life from damage received by player
    }

    IEnumerator enemyLoots()
    {
        yield return new WaitForSeconds(1F); // Wait for death animation to complete

        foreach (var item in loots) // Scroll through the loot lis
        {
            GameObject loot = Instantiate(item, transform.position, transform.localRotation); // Show loots
            Vector2 lootForce = new Vector2(Random.Range(-25, 25), Random.Range(75, 100)); // Loot force effects

            loot.GetComponent<Rigidbody2D>().AddForce(lootForce); // Add force in loot
        }

        Destroy(gameObject); // Destroy enemy object
    }

    IEnumerator enemyImmune()
    {
        // Change color to red for next 0.4F seconds
        enemySpriteRenderer.color = enemyStatesColor[1];
        yield return new WaitForSeconds(0.4F);

        // Set default state to enemy to be able hit it again
        enemySpriteRenderer.color = enemyStatesColor[0];

        // Change enemy states to VULNERABLE if it is not death
        if (enemyHitState == EnemyHitState.IMMUNE) enemyHitState = EnemyHitState.VULNERABLE;

        // Set enemy health bar to hidden
        enemyHPBarConfig.SetActive(false);
    }
}
