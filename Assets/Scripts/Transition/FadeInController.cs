using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInController : MonoBehaviour
{

    public GameObject panelTransition;
    public Image imageTransition;
    public Color[] colorTransition;
    public float stepTransition;

    public void fadeIn()
    {
        panelTransition.SetActive(true);  // Enable canvas transition
        StartCoroutine(fadeI()); // Start transition animation
    }

    public void fadeOut()
    {
        StartCoroutine(fadeO()); // Start transition animation
    }

    IEnumerator fadeI()
    {
        for (float i = 0; i <= 1; i += stepTransition)
        {
            imageTransition.color = Color.Lerp(colorTransition[0], colorTransition[1], i);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator fadeO()
    {
        for (float i = 0; i <= 1; i += stepTransition)
        {
            imageTransition.color = Color.Lerp(colorTransition[1], colorTransition[0], i);
            yield return new WaitForEndOfFrame();
        }

        panelTransition.SetActive(false);  // Disable canvas transition

    }
}
