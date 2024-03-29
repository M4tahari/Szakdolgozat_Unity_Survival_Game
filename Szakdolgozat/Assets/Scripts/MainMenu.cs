using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static int _sceneIndex;
    public void CreateNewWorld()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void LoadExistingWorlds()
    {
        _sceneIndex = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
    public void CheckSettings()
    {
        _sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
