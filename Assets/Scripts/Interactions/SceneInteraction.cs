using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInteraction : MonoBehaviour
{

    public FadeInController fadeController;
    public GameController gameController;

    public string sceneDestination;

    void Start()
    {
        fadeController = FindObjectOfType(typeof(FadeInController)) as FadeInController;
        gameController = FindObjectOfType(typeof(GameController)) as GameController;
    }

    public void interaction()
    {
        StartCoroutine(changeScene());
    }

    IEnumerator changeScene()
    {
        fadeController.fadeIn();

        yield return new WaitWhile(() => fadeController.imageTransition.color.a < 0.9F);

        if (sceneDestination == "Menu")
        {
            Destroy(gameController.gameObject); // After finish game or back to menu, destroy game controller
        }

        SceneManager.LoadScene(sceneDestination);
    }
}
