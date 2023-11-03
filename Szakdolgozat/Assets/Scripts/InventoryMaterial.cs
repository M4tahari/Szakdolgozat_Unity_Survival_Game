using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryMaterial : InventoryCountable
{
    [HideInInspector] public bool isOverMaterial = false;
    public void Update()
    {
        isOverMaterial = (Input.mousePosition.x >= gameObject.transform.position.x - 16 &&
        Input.mousePosition.x <= gameObject.transform.position.x + 16) &&
        (Input.mousePosition.y >= gameObject.transform.position.y - 16 &&
        Input.mousePosition.y <= gameObject.transform.position.y + 16);
    }
}
