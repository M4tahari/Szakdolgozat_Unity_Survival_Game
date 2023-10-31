using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] inventorySlots;
    public InventorySlot[] craftingStationSlots;
    public CraftingResultSlot craftingResultSlot;
    public InventorySlot deleteSlot;
    public GameObject inventoryItemPrefab;
    public GameObject inventoryWeaponPrefab;
    public GameObject inventoryMaterialPrefab;
    private InventoryItem itemInSlot;
    private InventoryWeapon weaponInSlot;
    private InventoryMaterial materialInSlot;
    public Item stick;
    public GameObject map;
    public Player player;
    [HideInInspector] public bool canPlace = true;

    public int selectedSlot = -1;
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

        for(int i = 0;i < craftingStationSlots.Length; i++)
        {
            craftingStationSlots[i].Select();
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

        if (player.weapons != null)
        {
            foreach (KeyValuePair<SerializableDictionary<Item, Weapon>, int> invWeapon in player.weapons)
            {
                foreach (var a in invWeapon.Key)
                {
                    SpawnNewWeapon(a.Key, inventorySlots[invWeapon.Value], a.Value);
                }
            }
        }

        if (player.materials!= null)
        {
            foreach (KeyValuePair<Item, SerializableDictionary<int, int>> invMaterial in player.materials)
            {
                foreach (var a in invMaterial.Value)
                {
                    SpawnNewMaterial(invMaterial.Key, inventorySlots[a.Key], a.Value);
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

        if(deleteSlot.transform.childCount > 0)
        {
            Destroy(deleteSlot.transform.GetChild(0).gameObject);
        }

        player.items = GatherAllItems();
        player.weapons = GatherAllWeapons();
        player.materials = GatherAllMaterials(); 

        Crafting();
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
               weaponInSlot = slot.GetComponentInChildren<InventoryWeapon>();
               materialInSlot = slot.GetComponentInChildren<InventoryMaterial>();
            }
            
            if (itemInSlot != null && weaponInSlot == null && materialInSlot == null && itemInSlot.item == item && itemInSlot.count < itemInSlot.item.stackAmount)
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
                weaponInSlot = slot.GetComponentInChildren<InventoryWeapon>();
                materialInSlot = slot.GetComponentInChildren<InventoryMaterial>();
            }
           
            if(itemInSlot == null && weaponInSlot == null && materialInSlot == null)
            {
                SpawnNewItem(item, slot, destroyable, item.currentAmount);
                return true;
            }
        }

        return false;
    }
    public bool AddWeapon(Item item, Weapon weapon)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            if (slot)
            {
                itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                weaponInSlot = slot.GetComponentInChildren<InventoryWeapon>();
                materialInSlot = slot.GetComponentInChildren<InventoryMaterial>();
            }

            if (weaponInSlot != null && itemInSlot == null && materialInSlot == null && weaponInSlot.item == item)
            {
                return true;
            }
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            if (slot)
            {
                itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                weaponInSlot = slot.GetComponentInChildren<InventoryWeapon>();
                materialInSlot = slot.GetComponentInChildren<InventoryMaterial>();
            }

            if (weaponInSlot == null && itemInSlot == null && materialInSlot == null)
            {
                SpawnNewWeapon(item, slot, weapon);
                return true;
            }
        }

        return false;
    }
    public bool AddMaterial(Item item)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            if (slot)
            {
                itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                weaponInSlot = slot.GetComponentInChildren<InventoryWeapon>();
                materialInSlot = slot.GetComponentInChildren<InventoryMaterial>();
            }

            if (materialInSlot != null && itemInSlot == null && weaponInSlot == null && materialInSlot.item == item && materialInSlot.count < materialInSlot.item.stackAmount)
            {
                materialInSlot.count++;
                materialInSlot.item.currentAmount++;
                materialInSlot.RefreshCount();
                return true;
            }
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            if (slot)
            {
                itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                weaponInSlot = slot.GetComponentInChildren<InventoryWeapon>();
                materialInSlot = slot.GetComponentInChildren<InventoryMaterial>();
            }

            if (materialInSlot == null && itemInSlot == null && weaponInSlot == null)
            {
                SpawnNewMaterial(item, slot, item.currentAmount);
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
            newItemObject = Instantiate(inventoryItemPrefab, slot.transform);
            InventoryItem inventoryItem = newItemObject.GetComponent<InventoryItem>();
            inventoryItem.destroyable = destroyable;
            inventoryItem.count = currentAmount;
            inventoryItem.InitializeItem(item);
        }
    }
    private void CraftNewMaterial(Item item, CraftingResultSlot slot, int currentAmount)
    {
        if (slot)
        {
            GameObject newItemObject;
            newItemObject = Instantiate(inventoryMaterialPrefab, slot.transform);
            InventoryMaterial inventoryMaterial = newItemObject.GetComponent<InventoryMaterial>();
            inventoryMaterial.count = currentAmount;
            inventoryMaterial.InitializeMaterial(item);
        }
    }
    private void SpawnNewMaterial(Item item, InventorySlot slot, int currentAmount)
    {
        if (slot)
        {
            GameObject newItemObject;
            newItemObject = Instantiate(inventoryMaterialPrefab, slot.transform);
            InventoryMaterial inventoryMaterial = newItemObject.GetComponent<InventoryMaterial>();
            inventoryMaterial.count = currentAmount;
            inventoryMaterial.InitializeMaterial(item);
        }
    }
    private void SpawnNewWeapon(Item item, InventorySlot slot, Weapon weapon)
    {
        if (slot)
        {
            GameObject newWeaponObject;
            newWeaponObject = Instantiate(inventoryWeaponPrefab, slot.transform);
            InventoryWeapon inventoryWeapon = newWeaponObject.GetComponent<InventoryWeapon>();
            inventoryWeapon.weapon = weapon;
            inventoryWeapon.InitializeItem(item);
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

        for (int i = 0; i < craftingStationSlots.Length; i++)
        {
            InventorySlot slot = craftingStationSlots[i];

            if (slot)
            {
                InventoryItem item = slot.GetComponentInChildren<InventoryItem>();
                SerializableDictionary<Item, Destroyable> data = new SerializableDictionary<Item, Destroyable>();
                SerializableDictionary<int, int> data2 = new SerializableDictionary<int, int>();
                if (item != null)
                {
                    data.Add(item.item, item.destroyable);
                    data2.Add(i, item.count);
                }

                items.Add(data, data2);
            }
        }

        return items;
    }
    public SerializableDictionary<SerializableDictionary<Item, Weapon>, int> GatherAllWeapons()
    {
        SerializableDictionary<SerializableDictionary<Item, Weapon>, int> weapons = new SerializableDictionary<SerializableDictionary<Item, Weapon>, int>();

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];

            if (slot)
            {
                InventoryWeapon weapon = slot.GetComponentInChildren<InventoryWeapon>();
                SerializableDictionary<Item, Weapon> data = new SerializableDictionary<Item, Weapon>();
                if (weapon != null)
                {
                    data.Add(weapon.item, weapon.weapon);
                }

                weapons.Add(data, i);
            }
        }

        for (int i = 0; i < craftingStationSlots.Length; i++)
        {
            InventorySlot slot = craftingStationSlots[i];

            if (slot)
            {
                InventoryWeapon weapon = slot.GetComponentInChildren<InventoryWeapon>();
                SerializableDictionary<Item, Weapon> data = new SerializableDictionary<Item, Weapon>();
                if (weapon != null)
                {
                    data.Add(weapon.item, weapon.weapon);
                }

                weapons.Add(data, i);
            }
        }

        return weapons;
    }
    public SerializableDictionary<Item, SerializableDictionary<int, int>> GatherAllMaterials()
    {
        SerializableDictionary<Item, SerializableDictionary<int, int>> materials = new SerializableDictionary<Item, SerializableDictionary<int, int>>();

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];

            if (slot)
            {
                InventoryMaterial material = slot.GetComponentInChildren<InventoryMaterial>();
                SerializableDictionary<int, int> data = new SerializableDictionary<int, int>();
                if (material != null)
                {
                    data.Add(i, material.item.currentAmount);
                    materials.Add(material.item, data);
                }
            }
        }

        for (int i = 0; i < craftingStationSlots.Length; i++)
        {
            InventorySlot slot = craftingStationSlots[i];

            if (slot)
            {
                InventoryMaterial material = slot.GetComponentInChildren<InventoryMaterial>();
                SerializableDictionary<int, int> data = new SerializableDictionary<int, int>();
                if (material != null)
                {
                    data.Add(i, material.item.currentAmount);
                    materials.Add(material.item, data);
                }
            }
        }

        return materials;
    }
    public void Crafting()
    {
        bool stickCraft = false;

        for(int i = 0; i < craftingStationSlots.Length; i++)
        {
            InventorySlot slot = craftingStationSlots[i];

            if (slot)
            {
                InventorySlot[] allButOne = craftingStationSlots.Skip(i).ToArray();

                for(int j = 0; j < allButOne.Length; j++)
                {
                    if(allButOne[j].transform.childCount == 0)
                    {
                        stickCraft = true;
                    }

                    else
                    {
                        stickCraft = false;
                    }
                }

                if(slot.transform.GetComponent<InventoryItem>() != null)
                {
                    if (stickCraft == true && (slot.transform.GetComponent<InventoryItem>().item.itemName == "Jungle tree log" ||
                    slot.transform.GetComponent<InventoryItem>().item.itemName == "Wetlands tree log"))
                    {
                        CraftNewMaterial(stick, craftingResultSlot, slot.transform.GetComponent<InventoryItem>().item.currentAmount * 2);
                        stickCraft = false;
                    }

                    if (craftingResultSlot.transform.childCount == 0)
                    {
                        DeleteItem(slot.transform.GetComponentInChildren<InventoryItem>());
                    }
                }
            }
        }
    }
}
