using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class Destroyable : Interactable
{
    public float hittingTimer = 0;
    public float requiredTime;
    private float requiredTime2;
    private bool breakable = true;
    public Player player;
    private float distance = 0;
    public InventoryManager inventoryManager;
    public Item itemToPickup;
    [HideInInspector] private bool quit = false;
    private void Update()
    {
        if(player != null)
        {
            distance = Vector2.Distance(player.transform.position, transform.position);
        }
        
        if (breakable == false && distance <= player.pickupRadius)
        {
            Destroy(gameObject);
        }
    }
    private void OnMouseDrag()
    {
        if (distance <= radius)
        {
            hittingTimer += Time.deltaTime;
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

            bool result = inventoryManager.AddItem(temp.itemToPickup, newBlock);
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
