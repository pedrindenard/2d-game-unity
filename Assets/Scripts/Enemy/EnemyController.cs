using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    [Header("Enemy controller")]
    private GameController gameController;
    private PlayerController playerController;
    private SpriteRenderer enemySpriteRenderer;

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

    void Start()
    {
        findControllers();
        findEnemyLookingState();
        findEnemyComponents();
        findEnemyInitialStates();
    }

    void Update()
    {
        checkPlayerPosition();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        switch (collider.gameObject.tag)
        {
            case "Weapon":

                if (enemyHitState == EnemyHitState.VULNERABLE)
                {
                    enemyHitState = EnemyHitState.IMMUNE; // Set to IMMUNE for block double attack when animation git is running
                    enemyHPBarConfig.SetActive(true); // Show enemy health bar

                    WeaponInformation weapon = collider.gameObject.GetComponent<WeaponInformation>();

                    int weaponDamageType = weapon.weaponDamageType;
                    float weaponDamage = weapon.weaponDamage;
                    float weaponBonusDamage = weaponDamage + (weaponDamage * (enemyDamageAdjustments[weaponDamageType] / 100));

                    enemyCurrentLife -= Mathf.RoundToInt(weaponBonusDamage); // Reduce enemy life from damage received by player
                    setEnemyHealthBarPercent(); // Set enemy health bar current life
                    killEnemyIfThereNoMoreLife(); // Destroy this game object if enemy life is 0

                    GameObject knockTemp = Instantiate(knockForcePrefab, knockPosition.position, knockPosition.localRotation);
                    Destroy(knockTemp, 0.03F); // Destroy knock object after 0.03s because engine physics is 0.02s

                    StartCoroutine("enemyImmune"); // Change sprite renderer color to red to indicate that player hit damage
                }

                break;
        }
    }

    void findControllers()
    {
        gameController = FindObjectOfType(typeof(GameController)) as GameController;
        playerController = FindObjectOfType(typeof(PlayerController)) as PlayerController;
    }

    void findKnockXPosition(float x)
    {
        knockPosition.localPosition = new Vector3(x, knockPosition.localPosition.y, 0);
    }

    void findEnemyComponents()
    {
        enemySpriteRenderer = GetComponent<SpriteRenderer>();
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

    void findEnemyLookingState()
    {
        if (enemyLookingState == EnemyLookingState.RIGHT)
        {
            flipEnemy();
        }
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
        findKnockXPosition(knockXTemp);
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
            Destroy(gameObject);
        }
    }

    IEnumerator enemyImmune()
    {
        // Change color to red for next 0.2F seconds
        enemySpriteRenderer.color = enemyStatesColor[1];
        yield return new WaitForSeconds(0.4F);

        // Set default state to enemy to be able hit it again
        enemySpriteRenderer.color = enemyStatesColor[0];
        enemyHitState = EnemyHitState.VULNERABLE;

        // Set enemy health bar to hidden
        enemyHPBarConfig.SetActive(false);
    }
}
