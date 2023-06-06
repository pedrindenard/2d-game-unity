using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : MonoBehaviour
{

    [Header("Controllers")]
    public FadeInController fadeController;
    public PlayerController playerController;

    [Header("Transforms")]
    public Transform playerTransform;
    public Transform playerDestination;

    [Header("Materials light 2D")]
    public Material defaultMaterial;
    public Material lightMaterial;

    public AmbientLightStates ambientLight;

    void Start()
    {
        fadeController = FindObjectOfType(typeof(FadeInController)) as FadeInController;
        playerController = FindObjectOfType(typeof(PlayerController)) as PlayerController;
    }

    void findPlayerTransform()
    {
        playerTransform = playerController.transform;
    }

    void interaction()
    {
        StartCoroutine(triggerDoorEvent());
    }

    IEnumerator triggerDoorEvent()
    {
        fadeController.fadeIn(); // Start fade transition

        yield return new WaitWhile(() => fadeController.imageTransition.color.a < 0.9F); // Do fade animation changing color alpha
        playerController.transform.position = playerDestination.position; // Set player destination

        setPlayerLight(); // Changes player sensitive to light

        fadeController.fadeOut(); // End fade transition
    }

    void setPlayerLight()
    {
        switch (ambientLight)
        {
            case AmbientLightStates.NIGHT:
                playerController.setPlayerMaterial2D(lightMaterial); // Make the player sensitive to light
                break;

            case AmbientLightStates.LIGHT:
                playerController.setPlayerMaterial2D(defaultMaterial); // Make the player ignore light sensitive
                break;
        }
    }
}