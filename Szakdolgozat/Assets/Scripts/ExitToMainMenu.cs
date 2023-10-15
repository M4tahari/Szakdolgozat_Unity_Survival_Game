using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToMainMenu : MonoBehaviour
{
    public void Exit()
    {
        if(MainMenu._sceneIndex == 4)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 4);
        }

        else if(MainMenu._sceneIndex == 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
        }
    }
}
