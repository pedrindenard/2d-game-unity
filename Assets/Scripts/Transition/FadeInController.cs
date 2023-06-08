using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInController : MonoBehaviour
{

    public ActionRunningStates runningState = ActionRunningStates.STOP;

    public GameObject panelTransition;
    public Image imageTransition;

    public Color[] colorTransition;
    public float stepTransition;

    void Start()
    {
        fadeOut();
    }

    public void fadeIn()
    {
        if (runningState == ActionRunningStates.STOP)
        {
            setActive();  // Enable canvas transition
            StartCoroutine(fadeI()); // Start transition animation
        }
    }

    public void fadeOut()
    {
        setActive(); // Enable canvas transition
        StartCoroutine(fadeO()); // Start transition animation
    }

    void setActive()
    {
        if (!panelTransition.gameObject.activeSelf)
        {
            panelTransition.SetActive(true);
        }
    }

    IEnumerator fadeI()
    {
        runningState = ActionRunningStates.RUNNING;

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

        runningState = ActionRunningStates.STOP;
    }
}
