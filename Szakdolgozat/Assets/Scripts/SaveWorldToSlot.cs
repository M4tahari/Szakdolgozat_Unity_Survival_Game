using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveWorldToSlot : MonoBehaviour
{
    private string worldID = "";
    private string difficulty = "";
    private string size = "";
    [SerializeField] public GameObject noLoadedWorld;
    [SerializeField] public GameObject hasLoadedWorld;
    [SerializeField] private TextMeshProUGUI worldName;
    [SerializeField] private TextMeshProUGUI diffText;
    [SerializeField] private TextMeshProUGUI sizeText;
    [SerializeField] private bool isLoaded = false;

    private Button saveWorldSlotButton;
    private void Awake()
    {
        saveWorldSlotButton = this.GetComponent<Button>();
    }
    public void SetData(WorldState state)
    {
        if(state == null)
        {
            noLoadedWorld.SetActive(true);
            hasLoadedWorld.SetActive(false);
            SetIsLoaded(false);
        }

        else
        {
            noLoadedWorld.SetActive(false);
            hasLoadedWorld.SetActive(true);

            worldID = state.worldName;
            difficulty = state.difficulty;
            size = state.mapSize.ToString();
            worldName.text = worldID;
            SetDifficultyText(difficulty);
            SetSizeText(size);
            SetIsLoaded(true);
        }
    }
    public string GetWorldID()
    {
        return this.worldID;
    }
    public string GetDifficulty()
    {
        return this.difficulty;
    }
    public bool GetIsLoaded()
    {
        return this.isLoaded;
    }
    public void SetIsLoaded(bool isLoaded)
    {
        this.isLoaded = isLoaded;
    }
    public void SetInteractable(bool interactable)
    {
        saveWorldSlotButton.interactable = interactable;
    }
    public void OnSaveWorldSlotClicked()
    {
        if (this.GetIsLoaded() == false)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }

        else
        {
            MainMenu._sceneIndex = 4;
            InputTextHandler.worldName = this.GetWorldID();
            InputTextHandler.difficulty = this.GetDifficulty();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
        }
    }
    public void SetDifficultyText(string text)
    {
        if (text == "easy")
        {
            diffText.text = "Könnyű";
        }

        else if (text == "medium")
        {
            diffText.text = "Közepes";
        }

        else if (text == "hard")
        {
            diffText.text = "Nehéz";
        }
    }
    public void SetSizeText(string text)
    {
        if(text == "1000")
        {
            sizeText.text = "Kicsi (1000)";
        }

        if (text == "2000")
        {
            sizeText.text = "Közepes (2000)";
        }

        if (text == "4000")
        {
            sizeText.text = "Nagy (4000)";
        }
    }
}
