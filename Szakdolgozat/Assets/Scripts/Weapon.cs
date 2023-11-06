using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : Interactable
{
    public Player player;
    public InventoryManager inventoryManager;
    public Item itemToPickup;
    public float attackDamage;
    public float attackSpeed;
    private float lastAttack;
    public float pushForce;
    public float staminaDrain;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private ContactFilter2D filter;
    private Collider2D[] collisions = new Collider2D[10];
    private bool collidingWithEntity;
    private GameObject collidingEntity;
    [HideInInspector] private bool quit = false;
    public void Start()
    {
        boxCollider = this.GetComponent<BoxCollider2D>();
        anim = this.GetComponent<Animator>();
        Physics2D.IgnoreLayerCollision(11, 9);
        Physics2D.IgnoreLayerCollision(11, 8);
    }
    public void FixedUpdate()
    {
        boxCollider = this.GetComponent<BoxCollider2D>();
        collidingWithEntity = false;
        boxCollider.OverlapCollider(filter, collisions);

        for(int i = 0; i < collisions.Length; i++)
        {
            if (collisions[i] == null)
            {
                continue;
            }

            if (collisions[i].tag == "Termite" || collisions[i].tag == "Capybara")
            {
                collidingWithEntity = true;
                collidingEntity = collisions[i].transform.gameObject;
            }

            collisions[i] = null;
        }

        if(Input.GetMouseButton(0))
        {
            if(Time.time - lastAttack > attackSpeed)
            {
                lastAttack = Time.time;
                Attack();
            }
        }
    }
    public void Attack()
    {
        if(player != null)
        {
            if(player.stamina > staminaDrain)
            {
                player.stamina -= staminaDrain;
                anim.SetTrigger("Attack");
            }
        }

        if(collidingWithEntity)
        {
            DamageEntity(collidingEntity);
        }
    }
    public void PickupWeapon()
    {
        Weapon temp = this;
        if (!quit)
        {
            Weapon newWeapon = Instantiate(this);
            inventoryManager.AddWeapon(temp.itemToPickup, newWeapon);
        }
    }
    private void DamageEntity(GameObject entity)
    {
        if(entity != null)
        {
            Termite termite = entity.GetComponent<Termite>();
            Capybara capybara = entity.GetComponent<Capybara>();

            if(termite != null)
            {
                if(termite.currentHealthPoints > 0)
                {
                    termite.ReceiveDamage(attackDamage, this.transform.position, pushForce);
                }
            }

            if (capybara != null)
            {
                if (capybara.currentHealthPoints > 0)
                {
                    capybara.ReceiveDamage(attackDamage, this.transform.position, pushForce);
                }
            }
        }
    }
    protected void OnCollide(Collider2D coll)
    { 
        if(coll.tag == "Player")
        {
            return;
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
