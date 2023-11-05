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
    public GameObject knife;
    public GameObject sword;
    public GameObject spear;
    public GameObject axe;
    public GameObject pickaxe;
    [HideInInspector] public bool canPlace = true;
    private bool stickCraft = false;
    private bool knifeCraft = false;
    private bool swordCraft = false;
    private bool spearCraft = false;
    private bool axeCraft = false;
    private bool pickaxeCraft = false;
    private bool readyToCraft = false;

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
                        SpawnNewItems(a.Key, inventorySlots[b.Key], a.Value, b.Value);
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
            foreach (KeyValuePair<SerializableDictionary<int, int>, Item> invMaterial in player.materials)
            {
                foreach (var a in invMaterial.Key)
                {
                    SpawnNewMaterials(invMaterial.Value, inventorySlots[a.Key], a.Value);
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

        Separate();
        Crafting();
    }
    private void ChangeSelectedSlot(int slotId)
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
                SpawnNewItem(item, slot, destroyable);
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
                SpawnNewMaterial(item, slot);
                return true;
            }
        }

        return false;
    }
    private void SpawnNewItem(Item item, InventorySlot slot, Destroyable destroyable)
    {
        if (slot)
        {
            GameObject newItemObject;
            newItemObject = Instantiate(inventoryItemPrefab, slot.transform);
            InventoryItem inventoryItem = newItemObject.GetComponent<InventoryItem>();
            inventoryItem.destroyable = destroyable;
            inventoryItem.InitializeItem(item);
            inventoryItem.RefreshCount();
        }
    }
    private void SpawnNewItems(Item item, InventorySlot slot, Destroyable destroyable, int currentAmount)
    {
        if(slot)
        {
            GameObject newItemObject;
            newItemObject = Instantiate(inventoryItemPrefab, slot.transform);
            InventoryItem inventoryItem = newItemObject.GetComponent<InventoryItem>();
            inventoryItem.destroyable = destroyable;
            inventoryItem.count = currentAmount;
            inventoryItem.InitializeItem(item);
            inventoryItem.RefreshCount();
        }
    }
    private void CraftNewMaterial(Item item, CraftingResultSlot slot, int currentAmount)
    {
        if (slot)
        {
            GameObject newItemObject;
            newItemObject = Instantiate(inventoryMaterialPrefab, slot.transform);
            InventoryMaterial inventoryMaterial = newItemObject.GetComponent<InventoryMaterial>();

            if(currentAmount <= item.stackAmount)
            {
                inventoryMaterial.count = currentAmount;
                inventoryMaterial.RefreshCount();
            }

            else
            {
                inventoryMaterial.count = item.stackAmount;
                inventoryMaterial.RefreshCount();
            }
           
            inventoryMaterial.InitializeItem(item);
        }
    }
    private void CraftNewWeapon(Item item, CraftingResultSlot slot, Weapon weapon)
    {
        if(slot)
        {
            GameObject newWeaponObject;
            newWeaponObject = Instantiate(inventoryWeaponPrefab, slot.transform);
            InventoryWeapon inventoryWeapon = newWeaponObject.GetComponent<InventoryWeapon>();
            inventoryWeapon.weapon = weapon;
            inventoryWeapon.InitializeItem(item);
        }
    }
    private void SpawnNewMaterial(Item item, InventorySlot slot)
    {
        if (slot)
        {
            GameObject newItemObject;
            newItemObject = Instantiate(inventoryMaterialPrefab, slot.transform);
            InventoryMaterial inventoryMaterial = newItemObject.GetComponent<InventoryMaterial>();
            item.currentAmount = 1;
            inventoryMaterial.InitializeItem(item);
            inventoryMaterial.RefreshCount();
        }
    }
    private void SpawnNewMaterials(Item item, InventorySlot slot, int currentAmount)
    {
        if (slot)
        {
            GameObject newItemObject;
            newItemObject = Instantiate(inventoryMaterialPrefab, slot.transform);
            InventoryMaterial inventoryMaterial = newItemObject.GetComponent<InventoryMaterial>();
            inventoryMaterial.count = currentAmount;
            inventoryMaterial.InitializeItem(item);
            inventoryMaterial.RefreshCount();
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
        if(item.count + item2.count <= item.item.stackAmount)
        {
            item.count += item2.count;
            item.item.currentAmount += item2.item.currentAmount;
            item.RefreshCount();
            DeleteItem(item2);
        }

        else
        {
            item2.count -= (item.item.stackAmount - item.count);
            item2.RefreshCount();
            item.count = item.item.stackAmount;
            item.RefreshCount();
        }
    }
    public void SeparateItems(InventoryItem item)
    {
        if(item.isOverItem)
        {
            if(Input.GetKeyDown(KeyCode.V))
            {
                if(item.count > 1 && item.count % 2 == 0)
                {
                    item.count /= 2;
                    item.RefreshCount();

                    for(int i = 0; i < inventorySlots.Length; i++)
                    {
                        InventorySlot slot = inventorySlots[i];

                        if(slot != null && slot.transform.childCount == 0)
                        {
                            SpawnNewItems(item.item, slot, item.destroyable, item.count);
                            break;
                        }
                    }
                }

                else if(item.count > 1 && item.count % 2 == 1)
                {
                    item.count -= 1;
                    item.RefreshCount();

                    for (int i = 0; i < inventorySlots.Length; i++)
                    {
                        InventorySlot slot = inventorySlots[i];

                        if (slot != null && slot.transform.childCount == 0)
                        {
                            SpawnNewItems(item.item, slot, item.destroyable, 1);
                            break;
                        }
                    }
                }
            }
        }
    }
    public void SeparateMaterials(InventoryMaterial material)
    {
        if (material.isOverMaterial)
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                if (material.count > 1 && material.count % 2 == 0)
                {
                    material.count /= 2;
                    material.RefreshCount();

                    for (int i = 0; i < inventorySlots.Length; i++)
                    {
                        InventorySlot slot = inventorySlots[i];

                        if (slot != null && slot.transform.childCount == 0)
                        {
                            SpawnNewMaterials(material.item, slot, material.count);
                            break;
                        }
                    }
                }

                if (material.count > 1 && material.count % 2 == 1)
                {
                    material.count -= 1;
                    material.RefreshCount();

                    for (int i = 0; i < inventorySlots.Length; i++)
                    {
                        InventorySlot slot = inventorySlots[i];

                        if (slot != null && slot.transform.childCount == 0)
                        {
                            SpawnNewMaterials(material.item, slot, 1);
                            break;
                        }
                    }
                }
            }
        }
    }
    public void Separate()
    {
        InventoryItem[] allItemsInInventory = new InventoryItem[inventorySlots.Length];
        InventoryMaterial[] allMaterialsInInventory = new InventoryMaterial[inventorySlots.Length];

        for(int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];

            if(slot != null && slot.transform.GetComponentInChildren<InventoryItem>() != null)
            {
                allItemsInInventory[i] = slot.transform.GetComponentInChildren<InventoryItem>();
            }
        }

        for(int j = 0; j < allItemsInInventory.Length; j++)
        {
            if (allItemsInInventory[j] != null)
            {
                SeparateItems(allItemsInInventory[j]);
            }
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];

            if (slot != null && slot.transform.GetComponentInChildren<InventoryMaterial>() != null)
            {
                allMaterialsInInventory[i] = slot.transform.GetComponentInChildren<InventoryMaterial>();
            }
        }

        for (int j = 0; j < allMaterialsInInventory.Length; j++)
        {
            if (allMaterialsInInventory[j] != null)
            {
                SeparateMaterials(allMaterialsInInventory[j]);
            }
        }
    }
    public void MergeMaterials(InventoryMaterial material, InventoryMaterial material2)
    {
        if (material.count + material2.count <= material.item.stackAmount)
        {
            material.count += material2.count;
            material.item.currentAmount += material2.item.currentAmount;
            material.RefreshCount();
            DeleteMaterial(material2);
        }

        else
        {
            material2.count -= (material.item.stackAmount - material.count);
            material2.RefreshCount();
            material.count = material.item.stackAmount;
            material.RefreshCount();
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
    public void DeleteMaterial(InventoryMaterial inventoryMaterial)
    {
        Destroy(inventoryMaterial.gameObject);
    }
    public void DeleteWeapon(InventoryWeapon inventoryWeapon)
    {
        Destroy(inventoryWeapon.gameObject);
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
    public SerializableDictionary<SerializableDictionary<int, int>, Item> GatherAllMaterials()
    {
        SerializableDictionary<SerializableDictionary<int, int>, Item> materials = new SerializableDictionary<SerializableDictionary<int, int>, Item>();

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];

            if (slot)
            {
                InventoryMaterial material = slot.GetComponentInChildren<InventoryMaterial>();
                SerializableDictionary<int, int> data = new SerializableDictionary<int, int>();
                if (material != null)
                {
                    data.Add(i, material.count);
                    materials.Add(data, material.item);
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
                    data.Add(i, material.count);
                    materials.Add(data, material.item);
                }
            }
        }

        return materials;
    }
    public void Crafting()
    {
        CraftStick();
        CraftKnife();
        CraftSword();
        CraftSpear();
        CraftAxe();
        CraftPickaxe();
    }
    public void CraftStick()
    {
        int emptySlotCount = 0;

        for (int i = 0; i < craftingStationSlots.Length; i++)
        {
            InventorySlot slot = craftingStationSlots[i];

            if (slot)
            {
                InventorySlot[] allButOne = craftingStationSlots;

                for (int j = 0; j < allButOne.Length; j++)
                {
                    if(j == i)
                    {
                        continue;
                    }

                    if (allButOne[j].transform.childCount == 0 && slot.transform.childCount == 1)
                    {
                        if (slot.transform.GetChild(0) != null)
                        {
                            if (slot.transform.GetChild(0).GetComponent<InventoryItem>() != null &&
                               (slot.transform.GetComponentInChildren<InventoryItem>().item.itemName == "Jungle tree log" ||
                               slot.transform.GetComponentInChildren<InventoryItem>().item.itemName == "Wetlands tree log"))
                            {
                                stickCraft = true;
                                knifeCraft = false;
                                swordCraft = false;
                                spearCraft = false;
                                axeCraft = false;
                                pickaxeCraft = false;
                            }
                        }
                    }
                }

                if(slot.transform.childCount == 0)
                {
                    emptySlotCount++;
                }

                if(emptySlotCount == 9)
                {
                    stickCraft = false;
                    readyToCraft = false;
                    emptySlotCount = 0;
                }

                if (stickCraft == false && readyToCraft == false)
                {
                    CheckToDeleteMaterial(craftingResultSlot);
                }

                else if (stickCraft == true && knifeCraft == false && swordCraft == false && spearCraft == false && 
                        axeCraft == false && pickaxeCraft == false && readyToCraft == false)
                {
                    CraftNewMaterial(stick, craftingResultSlot, slot.transform.GetChild(0).GetComponent<InventoryItem>().count * 2);
                    stickCraft = false;
                    knifeCraft = false;
                    swordCraft = false;
                    spearCraft = false;
                    axeCraft = false;
                    pickaxeCraft = false;
                    readyToCraft = true;
                }

                if (craftingResultSlot.transform.childCount == 0 && readyToCraft == true)
                {
                    if (slot.transform.childCount == 1)
                    {
                        if (slot.transform.GetChild(0) != null)
                        {
                            if (slot.transform.GetChild(0).GetComponent<InventoryItem>() != null)
                            {
                                DeleteItem(slot.transform.GetChild(0).GetComponentInChildren<InventoryItem>());
                                stickCraft = false;
                                knifeCraft = false;
                                swordCraft= false;
                                spearCraft = false;
                                axeCraft = false;
                                pickaxeCraft = false;
                                readyToCraft = false;
                            }
                        }
                    }
                }
            }
        }
    }
    public void CraftKnife()
    {
        for (int i = 0; i < craftingStationSlots.Length; i++)
        {
            if(i == 5 || i == 8)
            {
                continue;
            }

            InventorySlot slot = craftingStationSlots[i];

            if(slot)
            {
                if (slot.transform.childCount == 0 && craftingStationSlots[5].transform.childCount == 1 &&
                                craftingStationSlots[8].transform.childCount == 1)
                {
                    if (craftingStationSlots[5].transform.GetChild(0) != null && craftingStationSlots[8].transform.GetChild(0) != null)
                    {
                        if (craftingStationSlots[5].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                            craftingStationSlots[8].transform.GetChild(0).GetComponent<InventoryMaterial>() != null)
                        {
                            if (craftingStationSlots[5].transform.GetComponentInChildren<InventoryMaterial>().item.itemName == "Pebble" && 
                                craftingStationSlots[8].transform.GetComponentInChildren<InventoryMaterial>().item.itemName == "Stick")
                            {
                                knifeCraft = true;
                                stickCraft = false;
                                swordCraft = false;
                                spearCraft = false;
                                axeCraft = false;
                                pickaxeCraft = false;
                            }
                        }
                    }
                }
            }
        }

        if(craftingResultSlot.transform.childCount == 1)
        {
            if(craftingResultSlot.transform.GetChild(0) != null)
            {
                if(craftingResultSlot.transform.GetChild(0).GetComponent<InventoryWeapon>() != null)
                {
                    if(craftingResultSlot.transform.GetChild(0).GetComponent<InventoryWeapon>().item.itemName == "Stone knife")
                    {
                        if (craftingStationSlots[5].transform.childCount == 0 || craftingStationSlots[8].transform.childCount == 0)
                        {
                            knifeCraft = false;
                            readyToCraft = false;
                        }
                    }
                }
            }
        }

        if (knifeCraft == false && readyToCraft == false)
        {
            CheckToDeleteWeapon(craftingResultSlot);
        }

        else if (knifeCraft == true && stickCraft == false && swordCraft == false && spearCraft == false &&
                axeCraft == false && pickaxeCraft == false && readyToCraft == false)
        {
            CraftNewWeapon(knife.transform.GetComponent<Weapon>().itemToPickup, craftingResultSlot, knife.transform.GetComponent<Weapon>());
            stickCraft = false;
            knifeCraft = false;
            swordCraft = false;
            spearCraft = false;
            axeCraft = false;
            pickaxeCraft = false;
            readyToCraft = true;
        }

        if (craftingResultSlot.transform.childCount == 0 && readyToCraft == true)
        {
            if (craftingStationSlots[5].transform.childCount == 1 && craftingStationSlots[8].transform.childCount == 1)
            {
                if (craftingStationSlots[5].transform.GetChild(0) != null && craftingStationSlots[8].transform.GetChild(0) != null)
                {
                    if (craftingStationSlots[5].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                        craftingStationSlots[8].transform.GetChild(0).GetComponent<InventoryMaterial>() != null)
                    {
                        if(craftingStationSlots[5].transform.GetChild(0).GetComponent<InventoryMaterial>().count == 1)
                        {
                            DeleteMaterial(craftingStationSlots[5].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>());
                        }

                        else
                        {
                            craftingStationSlots[5].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().count -= 1;
                            craftingStationSlots[5].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().RefreshCount();
                        }

                        if (craftingStationSlots[8].transform.GetChild(0).GetComponent<InventoryMaterial>().count == 1)
                        {
                            DeleteMaterial(craftingStationSlots[8].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>());
                        }

                        else
                        {
                            craftingStationSlots[8].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().count -= 1;
                            craftingStationSlots[8].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().RefreshCount();
                        }

                        knifeCraft = false;
                        stickCraft = false;
                        swordCraft = false;
                        spearCraft = false;
                        axeCraft = false;
                        pickaxeCraft = false;
                        readyToCraft = false;
                    }
                }
            }
        }
    }
    public void CraftSword()
    {
        for (int i = 0; i < craftingStationSlots.Length; i++)
        {
            if (i == 2 || i == 4 || i == 6)
            {
                continue;
            }

            InventorySlot slot = craftingStationSlots[i];

            if (slot)
            {
                if (slot.transform.childCount == 0 && craftingStationSlots[2].transform.childCount == 1 &&
                    craftingStationSlots[4].transform.childCount == 1 && craftingStationSlots[6].transform.childCount == 1)
                {
                    if (craftingStationSlots[2].transform.GetChild(0) != null && craftingStationSlots[4].transform.GetChild(0) != null &&
                        craftingStationSlots[6].transform.GetChild(0) != null)
                    {
                        if (craftingStationSlots[2].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                            craftingStationSlots[4].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                            craftingStationSlots[6].transform.GetChild(0).GetComponent<InventoryMaterial>() != null)
                        {
                            if (craftingStationSlots[2].transform.GetComponentInChildren<InventoryMaterial>().item.itemName == "Pebble" &&
                                craftingStationSlots[4].transform.GetComponentInChildren<InventoryMaterial>().item.itemName == "Pebble" &&
                                craftingStationSlots[6].transform.GetComponentInChildren<InventoryMaterial>().item.itemName == "Stick")
                            {
                                swordCraft = true;
                                stickCraft = false;
                                knifeCraft = false;
                                spearCraft = false;
                                axeCraft = false;
                                pickaxeCraft = false;
                            }
                        }
                    }
                }
            }
        }

        if (craftingResultSlot.transform.childCount == 1)
        {
            if (craftingResultSlot.transform.GetChild(0) != null)
            {
                if (craftingResultSlot.transform.GetChild(0).GetComponent<InventoryWeapon>() != null)
                {
                    if (craftingResultSlot.transform.GetChild(0).GetComponent<InventoryWeapon>().item.itemName == "Stone sword")
                    {
                        if (craftingStationSlots[2].transform.childCount == 0 || craftingStationSlots[4].transform.childCount == 0 ||
                            craftingStationSlots[6].transform.childCount == 0)
                        {
                            swordCraft = false;
                            readyToCraft = false;
                        }
                    }
                }
            }
        }

        if (swordCraft == false && readyToCraft == false)
        {
            CheckToDeleteWeapon(craftingResultSlot);
        }

        else if (swordCraft == true && stickCraft == false && knifeCraft == false && spearCraft == false && 
            axeCraft == false && pickaxeCraft == false && readyToCraft == false)
        {
            CraftNewWeapon(sword.transform.GetComponent<Weapon>().itemToPickup, craftingResultSlot, sword.transform.GetComponent<Weapon>());
            stickCraft = false;
            knifeCraft = false;
            swordCraft = false;
            spearCraft = false;
            axeCraft = false;
            pickaxeCraft = false;
            readyToCraft = true;
        }

        if (craftingResultSlot.transform.childCount == 0 && readyToCraft == true)
        {
            if (craftingStationSlots[2].transform.childCount == 1 && craftingStationSlots[4].transform.childCount == 1 &&
                craftingStationSlots[6].transform.childCount == 1)
            {
                if (craftingStationSlots[2].transform.GetChild(0) != null && craftingStationSlots[4].transform.GetChild(0) != null &&
                    craftingStationSlots[6].transform.GetChild(0) != null)
                {
                    if (craftingStationSlots[2].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                        craftingStationSlots[4].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                        craftingStationSlots[4].transform.GetChild(0).GetComponent<InventoryMaterial>() != null)
                    {
                        if (craftingStationSlots[2].transform.GetChild(0).GetComponent<InventoryMaterial>().count == 1)
                        {
                            DeleteMaterial(craftingStationSlots[2].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>());
                        }

                        else
                        {
                            craftingStationSlots[2].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().count -= 1;
                            craftingStationSlots[2].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().RefreshCount();
                        }

                        if (craftingStationSlots[4].transform.GetChild(0).GetComponent<InventoryMaterial>().count == 1)
                        {
                            DeleteMaterial(craftingStationSlots[4].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>());
                        }

                        else
                        {
                            craftingStationSlots[4].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().count -= 1;
                            craftingStationSlots[4].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().RefreshCount();
                        }

                        if (craftingStationSlots[6].transform.GetChild(0).GetComponent<InventoryMaterial>().count == 1)
                        {
                            DeleteMaterial(craftingStationSlots[6].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>());
                        }

                        else
                        {
                            craftingStationSlots[6].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().count -= 1;
                            craftingStationSlots[6].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().RefreshCount();
                        }

                        swordCraft = false;
                        stickCraft = false;
                        knifeCraft = false;
                        spearCraft = false;
                        axeCraft = false;
                        pickaxeCraft = false;
                        readyToCraft = false;
                    }
                }
            }
        }
    }
    public void CraftSpear()
    {
        for (int i = 0; i < craftingStationSlots.Length; i++)
        {
            if (i == 2 || i == 4 || i == 6)
            {
                continue;
            }

            InventorySlot slot = craftingStationSlots[i];

            if (slot)
            {
                if (slot.transform.childCount == 0 && craftingStationSlots[2].transform.childCount == 1 &&
                    craftingStationSlots[4].transform.childCount == 1 && craftingStationSlots[6].transform.childCount == 1)
                {
                    if (craftingStationSlots[2].transform.GetChild(0) != null && craftingStationSlots[4].transform.GetChild(0) != null &&
                        craftingStationSlots[6].transform.GetChild(0) != null)
                    {
                        if (craftingStationSlots[2].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                            craftingStationSlots[4].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                            craftingStationSlots[6].transform.GetChild(0).GetComponent<InventoryMaterial>() != null)
                        {
                            if (craftingStationSlots[2].transform.GetComponentInChildren<InventoryMaterial>().item.itemName == "Pebble" &&
                                craftingStationSlots[4].transform.GetComponentInChildren<InventoryMaterial>().item.itemName == "Stick" &&
                                craftingStationSlots[6].transform.GetComponentInChildren<InventoryMaterial>().item.itemName == "Stick")
                            {
                                spearCraft = true;
                                stickCraft = false;
                                knifeCraft = false;
                                swordCraft = false;
                                axeCraft = false;
                                pickaxeCraft = false;
                            }
                        }
                    }
                }
            }
        }

        if (craftingResultSlot.transform.childCount == 1)
        {
            if (craftingResultSlot.transform.GetChild(0) != null)
            {
                if (craftingResultSlot.transform.GetChild(0).GetComponent<InventoryWeapon>() != null)
                {
                    if (craftingResultSlot.transform.GetChild(0).GetComponent<InventoryWeapon>().item.itemName == "Stone spear")
                    {
                        if (craftingStationSlots[2].transform.childCount == 0 || craftingStationSlots[4].transform.childCount == 0 ||
                            craftingStationSlots[6].transform.childCount == 0)
                        {
                            spearCraft = false;
                            readyToCraft = false;
                        }
                    }
                }
            }
        }

        if (spearCraft == false && readyToCraft == false)
        {
            CheckToDeleteWeapon(craftingResultSlot);
        }

        else if (spearCraft == true && stickCraft == false && knifeCraft == false && swordCraft == false && 
            axeCraft == false && pickaxeCraft == false && readyToCraft == false)
        {
            CraftNewWeapon(spear.transform.GetComponent<Weapon>().itemToPickup, craftingResultSlot, spear.transform.GetComponent<Weapon>());
            stickCraft = false;
            knifeCraft = false;
            swordCraft = false;
            spearCraft = false;
            axeCraft = false;
            pickaxeCraft = false;
            readyToCraft = true;
        }

        if (craftingResultSlot.transform.childCount == 0 && readyToCraft == true)
        {
            if (craftingStationSlots[2].transform.childCount == 1 && craftingStationSlots[4].transform.childCount == 1 &&
                craftingStationSlots[6].transform.childCount == 1)
            {
                if (craftingStationSlots[2].transform.GetChild(0) != null && craftingStationSlots[4].transform.GetChild(0) != null &&
                    craftingStationSlots[6].transform.GetChild(0) != null)
                {
                    if (craftingStationSlots[2].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                        craftingStationSlots[4].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                        craftingStationSlots[4].transform.GetChild(0).GetComponent<InventoryMaterial>() != null)
                    {
                        if (craftingStationSlots[2].transform.GetChild(0).GetComponent<InventoryMaterial>().count == 1)
                        {
                            DeleteMaterial(craftingStationSlots[2].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>());
                        }

                        else
                        {
                            craftingStationSlots[2].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().count -= 1;
                            craftingStationSlots[2].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().RefreshCount();
                        }

                        if (craftingStationSlots[4].transform.GetChild(0).GetComponent<InventoryMaterial>().count == 1)
                        {
                            DeleteMaterial(craftingStationSlots[4].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>());
                        }

                        else
                        {
                            craftingStationSlots[4].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().count -= 1;
                            craftingStationSlots[4].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().RefreshCount();
                        }

                        if (craftingStationSlots[6].transform.GetChild(0).GetComponent<InventoryMaterial>().count == 1)
                        {
                            DeleteMaterial(craftingStationSlots[6].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>());
                        }

                        else
                        {
                            craftingStationSlots[6].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().count -= 1;
                            craftingStationSlots[6].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().RefreshCount();
                        }

                        spearCraft = false;
                        stickCraft = false;
                        knifeCraft = false;
                        swordCraft = false;
                        axeCraft = false;
                        pickaxeCraft = false;
                        readyToCraft = false;
                    }
                }
            }
        }
    }
    public void CraftAxe()
    {
        for (int i = 0; i < craftingStationSlots.Length; i++)
        {
            if (i == 1 || i == 2 || i == 4 || i == 5 || i == 7)
            {
                continue;
            }

            InventorySlot slot = craftingStationSlots[i];

            if (slot)
            {
                if (slot.transform.childCount == 0 && craftingStationSlots[1].transform.childCount == 1 &&
                    craftingStationSlots[2].transform.childCount == 1 && craftingStationSlots[4].transform.childCount == 1 &&
                     craftingStationSlots[5].transform.childCount == 1 && craftingStationSlots[7].transform.childCount == 1)
                {
                    if (craftingStationSlots[1].transform.GetChild(0) != null && craftingStationSlots[2].transform.GetChild(0) != null &&
                        craftingStationSlots[4].transform.GetChild(0) != null && craftingStationSlots[5].transform.GetChild(0) != null &&
                        craftingStationSlots[7].transform.GetChild(0) != null)
                    {
                        if (craftingStationSlots[1].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                            craftingStationSlots[2].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                            craftingStationSlots[4].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                            craftingStationSlots[5].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                            craftingStationSlots[7].transform.GetChild(0).GetComponent<InventoryMaterial>() != null)
                        {
                            if (craftingStationSlots[1].transform.GetComponentInChildren<InventoryMaterial>().item.itemName == "Stick" &&
                                craftingStationSlots[2].transform.GetComponentInChildren<InventoryMaterial>().item.itemName == "Pebble" &&
                                craftingStationSlots[4].transform.GetComponentInChildren<InventoryMaterial>().item.itemName == "Stick" &&
                                craftingStationSlots[5].transform.GetComponentInChildren<InventoryMaterial>().item.itemName == "Pebble" &&
                                craftingStationSlots[7].transform.GetComponentInChildren<InventoryMaterial>().item.itemName == "Stick")
                            {
                                axeCraft = true;
                                stickCraft = false;
                                knifeCraft = false;
                                swordCraft = false;
                                spearCraft = false;
                                pickaxeCraft = false;
                            }
                        }
                    }
                }
            }
        }

        if (craftingResultSlot.transform.childCount == 1)
        {
            if (craftingResultSlot.transform.GetChild(0) != null)
            {
                if (craftingResultSlot.transform.GetChild(0).GetComponent<InventoryWeapon>() != null)
                {
                    if (craftingResultSlot.transform.GetChild(0).GetComponent<InventoryWeapon>().item.itemName == "Stone axe")
                    {
                        if (craftingStationSlots[1].transform.childCount == 0 || craftingStationSlots[2].transform.childCount == 0 ||
                            craftingStationSlots[4].transform.childCount == 0 || craftingStationSlots[5].transform.childCount == 0 ||
                            craftingStationSlots[7].transform.childCount == 0)
                        {
                            axeCraft = false;
                            readyToCraft = false;
                        }
                    }
                }
            }
        }

        if (axeCraft == false && readyToCraft == false)
        {
            CheckToDeleteWeapon(craftingResultSlot);
        }

        else if (axeCraft == true && stickCraft == false && knifeCraft == false && swordCraft == false &&
            spearCraft == false && pickaxeCraft == false && readyToCraft == false)
        {
            CraftNewWeapon(axe.transform.GetComponent<Weapon>().itemToPickup, craftingResultSlot, axe.transform.GetComponent<Weapon>());
            stickCraft = false;
            knifeCraft = false;
            swordCraft = false;
            spearCraft = false;
            axeCraft = false;
            pickaxeCraft = false;
            readyToCraft = true;
        }

        if (craftingResultSlot.transform.childCount == 0 && readyToCraft == true)
        {
            if (craftingStationSlots[1].transform.childCount == 1 && craftingStationSlots[2].transform.childCount == 1 &&
                craftingStationSlots[4].transform.childCount == 1 && craftingStationSlots[5].transform.childCount == 1 &&
                craftingStationSlots[7].transform.childCount == 1)
            {
                if (craftingStationSlots[1].transform.GetChild(0) != null && craftingStationSlots[2].transform.GetChild(0) != null &&
                    craftingStationSlots[4].transform.GetChild(0) != null && craftingStationSlots[5].transform.GetChild(0) != null &&
                    craftingStationSlots[7].transform.GetChild(0) != null)
                {
                    if (craftingStationSlots[1].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                        craftingStationSlots[2].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                        craftingStationSlots[4].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                        craftingStationSlots[5].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                        craftingStationSlots[7].transform.GetChild(0).GetComponent<InventoryMaterial>() != null)
                    {
                        if (craftingStationSlots[1].transform.GetChild(0).GetComponent<InventoryMaterial>().count == 1)
                        {
                            DeleteMaterial(craftingStationSlots[2].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>());
                        }

                        else
                        {
                            craftingStationSlots[1].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().count -= 1;
                            craftingStationSlots[1].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().RefreshCount();
                        }

                        if (craftingStationSlots[2].transform.GetChild(0).GetComponent<InventoryMaterial>().count == 1)
                        {
                            DeleteMaterial(craftingStationSlots[2].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>());
                        }

                        else
                        {
                            craftingStationSlots[2].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().count -= 1;
                            craftingStationSlots[2].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().RefreshCount();
                        }

                        if (craftingStationSlots[4].transform.GetChild(0).GetComponent<InventoryMaterial>().count == 1)
                        {
                            DeleteMaterial(craftingStationSlots[4].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>());
                        }

                        else
                        {
                            craftingStationSlots[4].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().count -= 1;
                            craftingStationSlots[4].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().RefreshCount();
                        }

                        if (craftingStationSlots[5].transform.GetChild(0).GetComponent<InventoryMaterial>().count == 1)
                        {
                            DeleteMaterial(craftingStationSlots[5].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>());
                        }

                        else
                        {
                            craftingStationSlots[5].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().count -= 1;
                            craftingStationSlots[5].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().RefreshCount();
                        }

                        if (craftingStationSlots[7].transform.GetChild(0).GetComponent<InventoryMaterial>().count == 1)
                        {
                            DeleteMaterial(craftingStationSlots[4].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>());
                        }

                        else
                        {
                            craftingStationSlots[7].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().count -= 1;
                            craftingStationSlots[7].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().RefreshCount();
                        }

                        spearCraft = false;
                        stickCraft = false;
                        knifeCraft = false;
                        swordCraft = false;
                        axeCraft = false;
                        pickaxeCraft = false;
                        readyToCraft = false;
                    }
                }
            }
        }
    }
    public void CraftPickaxe()
    {
        for (int i = 0; i < craftingStationSlots.Length; i++)
        {
            if (i == 0 || i == 1 || i == 2 || i == 4 || i == 5 || i == 6 || i == 8)
            {
                continue;
            }

            InventorySlot slot = craftingStationSlots[i];

            if (slot)
            {
                if (slot.transform.childCount == 0 && craftingStationSlots[0].transform.childCount == 1 &&
                    craftingStationSlots[1].transform.childCount == 1 && craftingStationSlots[2].transform.childCount == 1 &&
                    craftingStationSlots[4].transform.childCount == 1 && craftingStationSlots[5].transform.childCount == 1 &&
                    craftingStationSlots[6].transform.childCount == 1 && craftingStationSlots[8].transform.childCount == 1)
                {
                    if (craftingStationSlots[0].transform.GetChild(0) != null && craftingStationSlots[1].transform.GetChild(0) != null &&
                        craftingStationSlots[2].transform.GetChild(0) != null && craftingStationSlots[4].transform.GetChild(0) != null &&
                        craftingStationSlots[5].transform.GetChild(0) != null && craftingStationSlots[6].transform.GetChild(0) != null &&
                        craftingStationSlots[8].transform.GetChild(0) != null)
                    {
                        if (craftingStationSlots[0].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                            craftingStationSlots[1].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                            craftingStationSlots[2].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                            craftingStationSlots[4].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                            craftingStationSlots[5].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                            craftingStationSlots[6].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                            craftingStationSlots[8].transform.GetChild(0).GetComponent<InventoryMaterial>() != null)
                        {
                            if (craftingStationSlots[0].transform.GetComponentInChildren<InventoryMaterial>().item.itemName == "Pebble" &&
                                craftingStationSlots[1].transform.GetComponentInChildren<InventoryMaterial>().item.itemName == "Pebble" &&
                                craftingStationSlots[2].transform.GetComponentInChildren<InventoryMaterial>().item.itemName == "Stick" &&
                                craftingStationSlots[4].transform.GetComponentInChildren<InventoryMaterial>().item.itemName == "Stick" &&
                                craftingStationSlots[5].transform.GetComponentInChildren<InventoryMaterial>().item.itemName == "Pebble" &&
                                craftingStationSlots[6].transform.GetComponentInChildren<InventoryMaterial>().item.itemName == "Stick" &&
                                craftingStationSlots[8].transform.GetComponentInChildren<InventoryMaterial>().item.itemName == "Pebble")
                            {
                                pickaxeCraft = true;
                                stickCraft = false;
                                knifeCraft = false;
                                swordCraft = false;
                                spearCraft = false;
                                axeCraft = false;
                            }
                        }
                    }
                }
            }
        }

        if (craftingResultSlot.transform.childCount == 1)
        {
            if (craftingResultSlot.transform.GetChild(0) != null)
            {
                if (craftingResultSlot.transform.GetChild(0).GetComponent<InventoryWeapon>() != null)
                {
                    if (craftingResultSlot.transform.GetChild(0).GetComponent<InventoryWeapon>().item.itemName == "Stone pickaxe")
                    {
                        if (craftingStationSlots[0].transform.childCount == 0 || craftingStationSlots[1].transform.childCount == 0 ||
                            craftingStationSlots[2].transform.childCount == 0 || craftingStationSlots[4].transform.childCount == 0 ||
                            craftingStationSlots[5].transform.childCount == 0 || craftingStationSlots[6].transform.childCount == 0 ||
                            craftingStationSlots[8].transform.childCount == 0)
                        {
                            pickaxeCraft = false;
                            readyToCraft = false;
                        }
                    }
                }
            }
        }

        if (pickaxeCraft == false && readyToCraft == false)
        {
            CheckToDeleteWeapon(craftingResultSlot);
        }

        else if (pickaxeCraft == true && stickCraft == false && knifeCraft == false && swordCraft == false &&
            spearCraft == false && axeCraft == false && readyToCraft == false)
        {
            CraftNewWeapon(pickaxe.transform.GetComponent<Weapon>().itemToPickup, craftingResultSlot, pickaxe.transform.GetComponent<Weapon>());
            stickCraft = false;
            knifeCraft = false;
            swordCraft = false;
            spearCraft = false;
            axeCraft = false;
            pickaxeCraft = false;
            readyToCraft = true;
        }

        if (craftingResultSlot.transform.childCount == 0 && readyToCraft == true)
        {
            if (craftingStationSlots[0].transform.childCount == 1 && craftingStationSlots[1].transform.childCount == 1 &&
                craftingStationSlots[2].transform.childCount == 1 && craftingStationSlots[4].transform.childCount == 1 &&
                craftingStationSlots[5].transform.childCount == 1 && craftingStationSlots[6].transform.childCount == 1 &&
                craftingStationSlots[8].transform.childCount == 1)
            {
                if (craftingStationSlots[0].transform.GetChild(0) != null && craftingStationSlots[1].transform.GetChild(0) != null &&
                    craftingStationSlots[2].transform.GetChild(0) != null && craftingStationSlots[4].transform.GetChild(0) != null &&
                    craftingStationSlots[5].transform.GetChild(0) != null && craftingStationSlots[6].transform.GetChild(0) != null &&
                    craftingStationSlots[8].transform.GetChild(0) != null)
                {
                    if (craftingStationSlots[0].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                        craftingStationSlots[1].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                        craftingStationSlots[2].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                        craftingStationSlots[4].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                        craftingStationSlots[5].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                        craftingStationSlots[6].transform.GetChild(0).GetComponent<InventoryMaterial>() != null &&
                        craftingStationSlots[8].transform.GetChild(0).GetComponent<InventoryMaterial>() != null)
                    {
                        if (craftingStationSlots[0].transform.GetChild(0).GetComponent<InventoryMaterial>().count == 1)
                        {
                            DeleteMaterial(craftingStationSlots[0].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>());
                        }

                        else
                        {
                            craftingStationSlots[0].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().count -= 1;
                            craftingStationSlots[0].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().RefreshCount();
                        }

                        if (craftingStationSlots[1].transform.GetChild(0).GetComponent<InventoryMaterial>().count == 1)
                        {
                            DeleteMaterial(craftingStationSlots[1].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>());
                        }

                        else
                        {
                            craftingStationSlots[1].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().count -= 1;
                            craftingStationSlots[1].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().RefreshCount();
                        }

                        if (craftingStationSlots[2].transform.GetChild(0).GetComponent<InventoryMaterial>().count == 1)
                        {
                            DeleteMaterial(craftingStationSlots[2].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>());
                        }

                        else
                        {
                            craftingStationSlots[2].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().count -= 1;
                            craftingStationSlots[2].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().RefreshCount();
                        }

                        if (craftingStationSlots[4].transform.GetChild(0).GetComponent<InventoryMaterial>().count == 1)
                        {
                            DeleteMaterial(craftingStationSlots[4].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>());
                        }

                        else
                        {
                            craftingStationSlots[4].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().count -= 1;
                            craftingStationSlots[4].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().RefreshCount();
                        }

                        if (craftingStationSlots[5].transform.GetChild(0).GetComponent<InventoryMaterial>().count == 1)
                        {
                            DeleteMaterial(craftingStationSlots[5].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>());
                        }

                        else
                        {
                            craftingStationSlots[5].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().count -= 1;
                            craftingStationSlots[5].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().RefreshCount();
                        }

                        if (craftingStationSlots[6].transform.GetChild(0).GetComponent<InventoryMaterial>().count == 1)
                        {
                            DeleteMaterial(craftingStationSlots[6].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>());
                        }

                        else
                        {
                            craftingStationSlots[6].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().count -= 1;
                            craftingStationSlots[6].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().RefreshCount();
                        }

                        if (craftingStationSlots[8].transform.GetChild(0).GetComponent<InventoryMaterial>().count == 1)
                        {
                            DeleteMaterial(craftingStationSlots[8].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>());
                        }

                        else
                        {
                            craftingStationSlots[8].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().count -= 1;
                            craftingStationSlots[8].transform.GetChild(0).GetComponentInChildren<InventoryMaterial>().RefreshCount();
                        }

                        spearCraft = false;
                        stickCraft = false;
                        knifeCraft = false;
                        swordCraft = false;
                        axeCraft = false;
                        pickaxeCraft = false;
                        readyToCraft = false;
                    }
                }
            }
        }
    }
    public void CheckToDeleteMaterial(CraftingResultSlot slot)
    {
        if (slot.transform.childCount > 0)
        {
            if (slot.transform.GetChild(0) != null)
            {
                if (slot.transform.GetChild(0).GetComponent<InventoryMaterial>() != null)
                {
                    DeleteMaterial(slot.transform.GetChild(0).GetComponent<InventoryMaterial>());
                }
            }
        }
    }
    public void CheckToDeleteWeapon(CraftingResultSlot slot)
    {
        if (slot.transform.childCount > 0)
        {
            if (slot.transform.GetChild(0) != null)
            {
                if (slot.transform.GetChild(0).GetComponent<InventoryWeapon>() != null)
                {
                    DeleteWeapon(slot.transform.GetChild(0).GetComponent<InventoryWeapon>());
                }
            }
        }
    }
}


