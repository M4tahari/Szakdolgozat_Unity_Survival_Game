using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Interactable
{

    public Player player;
    public InventoryManager inventoryManager;
    public Item itemToPickup;
    public float attackDamage;
    public float attackSpeed;
    [HideInInspector] private bool quit = false;
    public void PickupWeapon()
    {
        Weapon temp = this;
        if (!quit)
        {
            Weapon newWeapon = Instantiate(this);
            inventoryManager.AddWeapon(temp.itemToPickup, newWeapon);
        }
    }
    public void OnApplicationQuit()
    {
        quit = true;
    }
    private void OnDestroy()
    {
        if (!this.gameObject.scene.isLoaded) return;
        PickupWeapon();
    }
}
