using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{

    private GameController gameController;

    private Rigidbody2D bossBody; // Physics
    private Animator bossAnim; // Animator to control animations
    private Transform bossTarget; // Indicates where boss need to move

    public BossLooking bossLooking; // Which direction boos is looking
    public int bossIdRoutine; // Routine id

    public bool isBossMoving; // Indicates if the boss is moving

    public float bossWaitTime; // How long to wait
    public float bossIdle; // Time in seconds boss will stay idle
    public float bossSpeed; // Boss movement velocity
    public int height; // Boss height

    public float[] enemyDamageAdjustments;
    public float enemyCurrentLife;

    public Transform[] waypoints; // Boss waypoints for select another routine
    public GameObject[] loots; // Boss loots

    void Start()
    {
        gameController = FindObjectOfType(typeof(GameController)) as GameController;

        bossAnim = GetComponent<Animator>();
        bossBody = GetComponent<Rigidbody2D>();

        // Initial boss routine
        bossIdRoutine = 0;
        
        initialRoutine();
    }

    void Update()
    {
        updateBossRoutine();
        updateBossMovement();
        updateBossLooking();
        updateAnimator();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        switch (collider.gameObject.tag)
        {
            case "Weapon":
                onDamageReceived(collider);
                break;

            case "Hit":
                onDamageReceived(collider);
                break;
        }
    }

    void updateBossRoutine()
    {
        switch (bossIdRoutine)
        {
            case 0:

                bossIdle += Time.deltaTime; // Wait for seconds using system clock

                if (bossIdle >= bossWaitTime)
                {
                    bossIdRoutine += 1; // Next routine

                    bossTarget = waypoints[3]; // Move to Limit D

                    height = -1; // Move to left

                    isBossMoving = true; // Set boss to move
                }

                break;

            case 1:

                if (transform.position.x <= bossTarget.position.x)
                {
                    bossIdRoutine += 1; // Next routine

                    bossTarget = waypoints[2]; // Move to Limit C

                    jumpBoss(); // Make boss jump
                }

                break;

            case 2:

                if (transform.position.x <= bossTarget.position.x)
                {
                    bossIdRoutine += 1; // Next routine
                    
                    bossTarget = waypoints[1]; // Move to Limit B

                    jumpBoss(); // Make boss jump
                }

                break;

            case 3:

                if (transform.position.x <= bossTarget.position.x)
                {
                    bossIdRoutine += 1; // Next routine
                    
                    bossTarget = waypoints[0]; // Move to Limit A
                }

                break;

            case 4:

                if (transform.position.x <= bossTarget.position.x)
                {
                    height = 0; // Remove boss movement

                    bossIdRoutine += 1; // Next routine

                    initialRoutine();
                }

                break;

            case 5:

                bossIdle += Time.deltaTime; // Wait for seconds using system clock

                if (bossIdle >= bossWaitTime)
                {
                    bossIdRoutine += 1; // Next routine

                    bossTarget = waypoints[1]; // Move to Limit B

                    height = 1; // Move to right
                }

                break;

            case 6:

                if (transform.position.x >= bossTarget.position.x)
                {
                    bossIdRoutine += 1; // Next routine

                    bossTarget = waypoints[2]; // Move to Limit C

                    jumpBoss(); // Make boss jump
                }

                break;

            case 7:

                if (transform.position.x >= bossTarget.position.x)
                {
                    bossIdRoutine += 1; // Next routine
                    
                    bossTarget = waypoints[3]; // Move to Limit D

                    jumpBoss(); // Make boss jump
                }

                break;

            case 8:

                if (transform.position.x >= bossTarget.position.x)
                {
                    bossIdRoutine += 1; // Next routine
                    
                    bossTarget = waypoints[4]; // Move to Limit E
                }

                break;
            
            case 9:

                if (transform.position.x >= bossTarget.position.x)
                {
                    height = 0; // Remove boss movement

                    bossIdRoutine = 0; // Reset routine

                    initialRoutine();
                }

                break;
        }
    }

    void updateBossLooking()
    {
        if (height > 0 && bossLooking == BossLooking.RIGHT)
        {
            flipBoss();
        }
        else if (height < 0 && bossLooking == BossLooking.LEFT)
        {
            flipBoss();
        }
    }

    void updateBossMovement()
    {
        if (isBossMoving)
        {
            bossBody.velocity = new Vector2(height * bossSpeed, bossBody.velocity.y);
        }
    }

    void updateAnimator()
    {
        bossAnim.SetInteger("height", height);
    }

    void flipBoss()
    {
        switch (bossLooking)
        {
            case BossLooking.RIGHT:
                bossLooking = BossLooking.LEFT;
                break;

            case BossLooking.LEFT:
                bossLooking = BossLooking.RIGHT;
                break;
        }

        float x = transform.localScale.x * -1;
        transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
    }

    void jumpBoss()
    {
        Vector2 jumpVector = new Vector2(0, 200);
        bossBody.AddForce(jumpVector);
    }

    void onDamageReceived(Collider2D collider)
    {
        bossAnim.SetTrigger("Hit"); // Show boss hit animation
        collider.gameObject.SendMessage("onDamageReceived", SendMessageOptions.DontRequireReceiver);

        WeaponInformation weapon = collider.gameObject.GetComponent<WeaponInformation>();

        int weaponDamageType = weapon.weaponDamageType;

        reduceEnemyLife(weaponDamageType, weapon); // Reduce boss health by damage attack received by player
        killEnemyIfThereNoMoreLife(); // Kill enemy and destroy object

        showHitEffect(weaponDamageType); // Show hit effect by weapon type
    }

    void reduceEnemyLife(int weaponDamageType, WeaponInformation weapon)
    {
        float weaponDamage = Random.Range(weapon.minWeaponDamage, weapon.maxWeaponDamage);;
        float weaponBonusDamage = weaponDamage + (weaponDamage * (enemyDamageAdjustments[weaponDamageType] / 100));

        enemyCurrentLife -= Mathf.RoundToInt(weaponBonusDamage); // Reduce enemy life from damage received by player
    }

    void killEnemyIfThereNoMoreLife()
    {
        if (enemyCurrentLife <= 0)
        {
            bossAnim.SetInteger("IdAnimation", 3); // Start enemy death animation
            
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

    void initialRoutine()
    {
        bossWaitTime = 3;
        bossIdle = 0;
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

        gameController.menuGameOver(); // Show game over

        Destroy(gameObject); // Destroy enemy object
    }
}
