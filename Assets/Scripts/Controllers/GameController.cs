using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{

    [Header("PLAYER SETTINGS")]
    public int idPerson;
    public int idPersonCurrent;
    public int maxLifePerson;
    public int idWeaponPerson;
    public int idWeaponPersonCurrent;

    [Header("PLAYER DB")]
    public string[] personNames;
    public Texture[] personSprites;

    [Header("WEAPON SETTINGS")]
    public string[] weaponDamageType;
    public GameObject[] weaponAttackEffects;

    [Header("WEAPON DB")]
    public Sprite[] weaponsStage1; // First stage of weapon animation
    public Sprite[] weaponsStage2; // Second stage of weapon animation
    public Sprite[] weaponsStage3; // Third stage of weapon animation
    public Sprite[] weaponsStage4; // Four stage of weapon animation

    public int[] weaponClasses; // Types of weapons, including combat (0), bows (1) and staffs (2)

    public int[] damageEffect;
    public int[] maxDamage;
    public int[] minDamage;

    public int[] cost;

    [Header("INVENTORY")]
    public TextMeshProUGUI playerCoinsGui;
    public int playerCoins;

    void Start()
    {
        DontDestroyOnLoad(gameObject); // Dont destroy game controller when scene changes
    }

    void Update()
    {
        updatePlayerCoins();
    }

    void updatePlayerCoins()
    {
        playerCoinsGui.text = playerCoins.ToString("N0").Replace(",", "."); // No decimal case and replace "," to ".", example: "2.000"
    }

    public void updatePlayerSkin()
    {
        idPersonCurrent = idPerson;
    }

    public Sprite weaponStage(WeaponStageStates stage, int id)
    {
        switch (stage) {
            case WeaponStageStates.ONE:
                return weaponsStage1[id];

            case WeaponStageStates.TWO:
                return weaponsStage2[id];

            case WeaponStageStates.THREE:
                return weaponsStage3[id];

            default:
                return weaponsStage4[id];
        }
    }
}