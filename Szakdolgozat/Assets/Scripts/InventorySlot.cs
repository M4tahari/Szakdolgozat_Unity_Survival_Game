using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image image;
    public float selected, notSelected;
    public void Select()
    {
        var temp = image.color;
        temp.a = selected;
        image.color = temp;
    }
    public void Deselect()
    {
        var temp = image.color;
        temp.a = notSelected;
        image.color = temp;
    }
    public void OnDrop(PointerEventData eventData)
    {
        if(transform.childCount == 0)
        {
            InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            inventoryItem.parentAfterDrag = transform;
        }
        
    }
}
