using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] inventorySlots;
    public InventorySlot DeleteSlot;
    public GameObject InventoryItemPrefab;
    private InventoryItem itemInSlot;
    public GameObject map;
    public Player player;
    [HideInInspector] public bool canPlace = true;

    int selectedSlot = -1;
    public void Awake()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i < 5)
            {
                inventorySlots[i].Deselect();
            }

            else
            {
                inventorySlots[i].Select();
            }
        }
    }
    public void Start()
    {
        ChangeSelectedSlot(0);

        if(player.items != null)
        {
            foreach(KeyValuePair<SerializableDictionary<Item, Destroyable>, SerializableDictionary<int, int>> invItem in player.items)
            {
                foreach(var a in invItem.Key)
                {
                    foreach(var b in invItem.Value)
                    {
                        SpawnNewItem(a.Key, inventorySlots[b.Key], a.Value, b.Value);
                    }
                }
            }
        }
    }
    public void Update()
    {
        if(Input.inputString != null)
        {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if (isNumber && number > 0 && number < 6)
            {
                ChangeSelectedSlot(number-1);
            }
        }

        if(DeleteSlot.transform.childCount > 0)
        {
            Destroy(DeleteSlot.transform.GetChild(0).gameObject);
        }

        player.items = GatherAllItems();
    }
    void ChangeSelectedSlot(int slotId)
    {
        if(selectedSlot >= 0)
        {
            inventorySlots[selectedSlot].Deselect();
        }

        inventorySlots[slotId].Select();
        selectedSlot = slotId;
    }
    public bool AddItem(Item item, Destroyable destroyable)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            if (slot)
            {
               itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            }
            
            if (itemInSlot != null && itemInSlot.item == item && itemInSlot.count < itemInSlot.item.stackAmount)
            {
                itemInSlot.count++;
                itemInSlot.item.currentAmount++;
                itemInSlot.RefreshCount();
                return true;
            }
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            if(slot)
            {
                itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            }
           
            if(itemInSlot == null)
            {
                SpawnNewItem(item, slot, destroyable, item.currentAmount);
                return true;
            }
        }

        return false;
    }
    private void SpawnNewItem(Item item, InventorySlot slot, Destroyable destroyable, int currentAmount)
    {
        if(slot)
        {
            GameObject newItemObject;
            newItemObject = Instantiate(InventoryItemPrefab, slot.transform);
            InventoryItem inventoryItem = newItemObject.GetComponent<InventoryItem>();
            inventoryItem.destroyable = destroyable;
            inventoryItem.count = currentAmount;
            inventoryItem.InitializeItem(item);
        }
    }
    public void MergeItems(InventoryItem item, InventoryItem item2)
    {
        if(item.item.currentAmount + item2.item.currentAmount <= item.item.stackAmount)
        {
            item.count += item2.count;
            item.item.currentAmount += item2.item.currentAmount;
            Destroy(item2);
        }
    }
    public void PlaceItem(InventoryItem inventoryItem)
    {
        inventoryItem.buildMode = false;

        if (canPlace)
        {
            Destroyable newDestroyable = Instantiate(inventoryItem.destroyable, map.transform);
            newDestroyable.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            newDestroyable.transform.position = new Vector3((float)Math.Round((newDestroyable.transform.position.x / 0.32f), MidpointRounding.ToEven) * 0.32f, 
                (float)Math.Round(newDestroyable.transform.position.y / 0.32f, MidpointRounding.ToEven) * 0.32f, 10);
            newDestroyable.transform.localScale = Vector3.one;
            Destroy(newDestroyable.GetComponent<Rigidbody2D>());
            newDestroyable.gameObject.SetActive(true);
            newDestroyable.GetComponent<Destroyable>().enabled = true;
            newDestroyable.GetComponent<BoxCollider2D>().enabled = true;

            if (inventoryItem.count == 1)
            {
                DeleteItem(inventoryItem);
            }

            else
            {
                inventoryItem.count--;
                inventoryItem.item.currentAmount--;
                inventoryItem.RefreshCount();
                inventoryItem.buildMode = true;
            }
        }

        else if(canPlace == false && Input.GetMouseButtonDown(0))
        {
            inventoryItem.buildMode = false;
            inventoryItem.clickCount = 0;
            inventoryItem.transform.position = inventoryItem.transform.parent.position;
        }
    }
    public void DeleteItem(InventoryItem inventoryItem)
    {
        Destroy(inventoryItem.gameObject);
    }
    public SerializableDictionary<SerializableDictionary<Item, Destroyable>, SerializableDictionary<int, int>> GatherAllItems()
    {
        SerializableDictionary<SerializableDictionary<Item, Destroyable>, SerializableDictionary<int, int>> items = new SerializableDictionary<SerializableDictionary<Item, Destroyable>, SerializableDictionary<int, int>>();

        for(int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];

            if(slot)
            {
                InventoryItem item = slot.GetComponentInChildren<InventoryItem>();
                SerializableDictionary<Item, Destroyable> data = new SerializableDictionary<Item, Destroyable>();
                SerializableDictionary<int, int> data2 = new SerializableDictionary<int, int>();
                if(item != null)
                {
                    data.Add(item.item, item.destroyable);
                    data2.Add(i, item.count);
                }

                items.Add(data, data2);
            }
        }

        return items;
    }
}
