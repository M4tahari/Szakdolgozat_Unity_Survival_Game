using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAppear : MonoBehaviour
{
    public GameObject menu;
    private bool isShowing = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isShowing = !isShowing;
            menu.SetActive(isShowing);
        }
    }
}
