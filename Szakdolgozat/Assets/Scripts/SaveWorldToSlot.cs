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
    [SerializeField] public GameObject noLoadedWorld;
    [SerializeField] public GameObject hasLoadedWorld;
    [SerializeField] private TextMeshProUGUI worldName;
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
            worldName.text = worldID;
            SetIsLoaded(true);
        }
    }
    public string GetWorldID()
    {
        return this.worldID;
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
        }
    }
}
