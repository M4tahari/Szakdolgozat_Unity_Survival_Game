using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryFood : InventoryCountable
{
    [HideInInspector] public bool isOverFood = false;
    public float foodValue;
    public float thirstValue;
    public void Update()
    {
        isOverFood = (Input.mousePosition.x >= gameObject.transform.position.x - 16 &&
        Input.mousePosition.x <= gameObject.transform.position.x + 16) &&
        (Input.mousePosition.y >= gameObject.transform.position.y - 16 &&
        Input.mousePosition.y <= gameObject.transform.position.y + 16);
    }
}
