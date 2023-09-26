using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAppear : MonoBehaviour
{
    public GameObject inventoryMenu;
    public GameObject escapeMenu;
    private bool isShowingInventory = false;
    private bool isShowingEscape = false;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            isShowingEscape = !isShowingEscape;
            escapeMenu.SetActive(isShowingEscape);
        }

        if(isShowingEscape == true)
        {
            Time.timeScale = 0.0f;
        }

        if(isShowingEscape == false)
        {
            Time.timeScale = 1.0f;
        }

        if (Input.GetKeyDown(KeyCode.I) && isShowingEscape == false)
        {
            isShowingInventory = !isShowingInventory;
            inventoryMenu.SetActive(isShowingInventory);
        }
    }
    public void BackToGame()
    {
        isShowingEscape = false;
        escapeMenu.SetActive(isShowingEscape);
        Time.timeScale = 1.0f;
    }
}
