using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsInputHandler : MonoBehaviour
{
    public static float renderDistanceMultiplier = 1.0f;
    public Slider renderDistanceSlider;

    public void Start()
    {
        if(PlayerPrefs.GetFloat("SliderValue", 0) > 0)
        {
            renderDistanceMultiplier = PlayerPrefs.GetFloat("SliderValue", 0);
            renderDistanceSlider.value = renderDistanceMultiplier;
        }

        else
        {
            renderDistanceMultiplier = 1.0f;
            renderDistanceSlider.value = renderDistanceMultiplier;
        }
    }
    public void SaveSettings()
    {
        renderDistanceMultiplier = renderDistanceSlider.value;
        PlayerPrefs.SetFloat("SliderValue", renderDistanceMultiplier);

        if (MainMenu._sceneIndex == 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 3);
        }

        if (MainMenu._sceneIndex == 4)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("SliderValue");
    }
}