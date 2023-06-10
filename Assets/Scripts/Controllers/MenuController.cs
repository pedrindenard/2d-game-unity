using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
   
    public void pickerPerson(int idPerson)
    {
        PlayerPrefs.SetInt("idPerson", idPerson); // Persist person id inside PLayerPrefs object
        SceneManager.LoadScene("Forest"); // Load next scene "MAP"
    }

}