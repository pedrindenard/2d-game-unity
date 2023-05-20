using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    private GameController gameController;

    public float[] enemyDamageAdjustments;

    void Start()
    {
        findGameController();
    }

    void Update()
    {
        
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

                print(weaponBonusDamage);
                print(gameController.weaponDamageType[weapon.weaponDamageType]);

                break;
        }
    }

    void findGameController()
    {
        if (gameController == null) gameController = FindObjectOfType(typeof(GameController)) as GameController;
    }
}
