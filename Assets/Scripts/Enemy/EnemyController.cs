using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    private GameController gameController;
    private PlayerController playerController;

    public float enemyLife;
    public float[] enemyDamageAdjustments;

    public EnemyLookingState enemyLookingState;
    public PlayerLookingState playerLookingState;

    public GameObject knockForcePrefab; // Repulsive force
    public Transform knockPosition; // Position force\

    private float knockXTemp; // Knock X position temp
    public float knockX; // Knock X position

    void Start()
    {
        findGameController();
        findPlayerController();
        findEnemyLookingState();
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
                
                WeaponInformation weapon = collider.gameObject.GetComponent<WeaponInformation>();

                int weaponDamageType = weapon.weaponDamageType;
                float weaponDamage = weapon.weaponDamage;

                float weaponBonusDamage = weaponDamage + (weaponDamage * (enemyDamageAdjustments[weaponDamageType] / 100));

                enemyLife -= Mathf.RoundToInt(weaponBonusDamage); // Reduce enemy life from received by player
                killEnemyIfThereNoMoreLife(); // Destroy this game object

                print(weaponBonusDamage);
                print(gameController.weaponDamageType[weapon.weaponDamageType]);

                GameObject knockTemp = Instantiate(knockForcePrefab, knockPosition.position, knockPosition.localRotation);

                Destroy(knockTemp, 0.03F); // Destroy knock object after 0.03s because engine physics is 0.02s

                break;
        }
    }

    void findGameController()
    {
        gameController = FindObjectOfType(typeof(GameController)) as GameController;
    }

    void findPlayerController()
    {
        playerController = FindObjectOfType(typeof(PlayerController)) as PlayerController;
    }

    void findKnockXPosition(float x)
    {
        knockPosition.localPosition = new Vector3(x, knockPosition.localPosition.y, 0);
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
        scaleX *= -1; // Invert "scaleX" value

        Vector3 scaleSettings = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);

        transform.localScale = scaleSettings; // Set enemy looking direction
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

    void killEnemyIfThereNoMoreLife()
    {
        if (enemyLife <= 0)
        {
            Destroy(gameObject);
        }
    }
}
