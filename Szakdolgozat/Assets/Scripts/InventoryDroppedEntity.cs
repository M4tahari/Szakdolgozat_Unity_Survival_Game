using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDroppedEntity : MonoBehaviour
{
    public Player player;
    private float distance = 0;
    public Image image;
    public Sprite sprite;
    public InventoryManager inventoryManager;
    public InventoryItem item;
    public InventoryWeapon weapon;
    public InventoryMaterial material;
    public InventoryFood food;
    [HideInInspector] private bool quit = false;
    private void Start()
    {
        image = this.GetComponent<Image>();

        if(item != null)
        {
            sprite = item.GetComponent<Sprite>();
            image.sprite = sprite;
            this.gameObject.transform.localScale = new Vector3(item.transform.localScale.x, item.transform.localScale.y, item.transform.localScale.z);
        }

        if (weapon != null)
        {
            sprite = weapon.GetComponent<Sprite>();
        }

        if (material != null)
        {
            sprite = material.GetComponent<Sprite>();
        }

        if (food != null)
        {
            sprite = food.GetComponent<Sprite>();
        }
    }
    private void Update()
    {
        if (player != null)
        {
            distance = Vector2.Distance(player.transform.position, transform.position);
        }

        if (distance <= player.pickupRadius)
        {
            Destroy(gameObject);
        }
    }
    public void OnApplicationQuit()
    {
        quit = true;
    }
    public void PickupEntity()
    {
        if (!quit)
        {
            for(int i = 0; i < inventoryManager.inventorySlots.Length; i++)
            {
                InventorySlot slot = inventoryManager.inventorySlots[i];

                if (item != null)
                {
                    inventoryManager.SpawnNewItems(item.item, slot, item.destroyable, item.count);
                }

                if (weapon != null)
                {
                    inventoryManager.SpawnNewWeapon(weapon.item, slot, weapon.weapon);
                }

                if (material != null)
                {
                    inventoryManager.SpawnNewMaterials(material.item, slot, material.count);
                }

                if (food != null)
                {
                    inventoryManager.SpawnNewFoods(food.item, slot, food.count, food.foodValue, food.thirstValue);
                }
            }

        }
    }
    private void OnDestroy()
    {
        if (!this.gameObject.scene.isLoaded) return;
        PickupEntity();
    }
}
