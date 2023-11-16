using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Destroyable : Interactable
{
    public float hittingTimer = 0;
    private float damage = 0;
    private SpriteRenderer spriteRenderer;
    private Material material;
    public float requiredTime;
    private float requiredTime2;
    private bool breakable = true;
    public Player player;
    private float distance = 0;
    public InventoryManager inventoryManager;
    public Item itemToPickup;
    public Item additionalMaterial;
    public Item additionalFood;
    public float foodValue;
    public float thirstValue;
    public bool hasAdditionalItem;
    public bool hasAdditionalFood;
    [HideInInspector] private bool quit = false;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
    }
    private void Update()
    {
        material.SetFloat("_Damage", damage);

        if (player != null)
        {
            distance = Vector2.Distance(player.transform.position, transform.position);
        }
        
        if (breakable == false && distance <= player.pickupRadius)
        {
            for(int i = 0; i < inventoryManager.inventorySlots.Length; i++)
            {
                InventorySlot slot = inventoryManager.inventorySlots[i];

                if(slot.transform.childCount == 0)
                {
                    Destroy(gameObject);
                    break;
                }
            }
        }
    }
    private void OnMouseDrag()
    {
        if (distance <= radius)
        {
            BreakFasterWithProperTools();
            hittingTimer += Time.deltaTime;
            damage += Time.deltaTime / requiredTime;
            requiredTime2 = requiredTime;

            if (hittingTimer > requiredTime2 && breakable)
            {
                breakable = false;
                hittingTimer = 0;
                requiredTime2 = 0;
                gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                gameObject.AddComponent<Rigidbody2D>();
            }
        }
    }
    private void OnMouseUp()
    {
        hittingTimer = 0;
        damage = 0;
        material.SetFloat("_Damage", damage);
    }
    public void OnMouseOver()
    { 
        inventoryManager.canPlace = false;
    }
    public void OnMouseExit()
    {
        inventoryManager.canPlace = true;
    }
    public void PickupItem()
    {
       Destroyable temp = this;
       if(!quit)
        {
            Destroyable newBlock = Instantiate(this);
            newBlock.breakable = true;
            inventoryManager.AddItem(temp.itemToPickup, newBlock);

            if(hasAdditionalItem)
            {
                var random = new System.Random();
                double multipleChance = random.NextDouble();

                if(multipleChance > 0.33f && multipleChance <= 0.66f)
                {
                    inventoryManager.AddMaterial(additionalMaterial);
                }

                else if(multipleChance > 0.66f)
                {
                    inventoryManager.AddMaterial(additionalMaterial);
                    inventoryManager.AddMaterial(additionalMaterial);
                }
            }

            if(hasAdditionalFood)
            {
                var random = new System.Random();
                double multipleChance = random.NextDouble();

                if (multipleChance > 0.33f && multipleChance <= 0.66f)
                {
                    inventoryManager.AddFood(additionalFood, foodValue, thirstValue);
                }

                else if (multipleChance > 0.66f)
                {
                    inventoryManager.AddFood(additionalFood, foodValue, thirstValue);
                    inventoryManager.AddFood(additionalFood, foodValue, thirstValue);
                }
            }
        }
    }
    public void BreakFasterWithProperTools()
    {
        if(itemToPickup.itemName == "Jungle tree log" || itemToPickup.itemName == "Wetlands tree log")
        {
            if(player.transform.GetChild(2).GetComponent<Weapon>() != null && 
                player.transform.GetChild(2).GetComponent<Weapon>().enabled == true)
            {
                hittingTimer += Time.deltaTime;
                damage += Time.deltaTime / requiredTime;
            }
        }

        if (itemToPickup.itemName == "Jungle grass block" || itemToPickup.itemName == "Dirt block" ||
            itemToPickup.itemName == "Hardened sand block" || itemToPickup.itemName == "Sand block" ||
            itemToPickup.itemName == "Termite castle wall" || itemToPickup.itemName == "Dirt block" ||
            itemToPickup.itemName == "Wetlands grass block" || itemToPickup.itemName == "Mud block")
        {
            if (player.transform.GetChild(3).GetComponent<Weapon>() != null &&
                player.transform.GetChild(3).GetComponent<Weapon>().enabled == true)
            {
                hittingTimer += Time.deltaTime;
                damage += Time.deltaTime / requiredTime;
            }
        }
    }
    public void OnApplicationQuit()
    {
        quit = true;
    }
    private void OnDestroy()
    {
        if (!this.gameObject.scene.isLoaded) return;
        PickupItem();
    }
}
