using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image image;
    public float selected, notSelected;
    public InventoryManager inventoryManager;
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
        InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
        InventoryWeapon inventoryWeapon = eventData.pointerDrag.GetComponent<InventoryWeapon>();
        InventoryMaterial inventoryMaterial = eventData.pointerDrag.GetComponent<InventoryMaterial>();

        if (transform.childCount == 0)
        {
            if (inventoryItem != null)
            {
                inventoryItem.parentAfterDrag = transform;
            }

            if(inventoryWeapon != null)
            {
                inventoryWeapon.parentAfterDrag = transform;
            }

            if(inventoryMaterial != null)
            {
                inventoryMaterial.parentAfterDrag = transform;
            }
        }

        else if(transform.childCount == 1)
        {
            InventoryItem currentItem = this.transform.GetChild(0).GetComponentInChildren<InventoryItem>();
            InventoryMaterial currentMaterial = this.transform.GetChild(0).GetComponentInChildren<InventoryMaterial>();

            if (currentItem != null && inventoryItem != null && currentItem.item.itemName == inventoryItem.item.itemName)
            {
                inventoryManager.MergeItems(currentItem, inventoryItem);
            }

            if(currentMaterial != null && inventoryMaterial != null && currentMaterial.item.itemName == inventoryMaterial.item.itemName)
            {
                inventoryManager.MergeMaterials(currentMaterial, inventoryMaterial);
            }
        }
    }
}
