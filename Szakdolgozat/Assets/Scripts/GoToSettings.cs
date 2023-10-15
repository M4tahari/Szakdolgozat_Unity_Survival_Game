using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToSettings : MonoBehaviour
{
    public void GoToSettingsMenu()
    {
        MainMenu._sceneIndex = 4;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
