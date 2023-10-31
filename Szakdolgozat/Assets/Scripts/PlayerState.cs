using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerState
{
    public float maxHealthPoints;
    public float currentHealthPoints;
    public float maxHungerPoints;
    public float currentHungerPoints;
    public float maxHydrationPoints;
    public float currentHydrationPoints;
    public float currentSpeed;
    public float exponentialPenalty;
    public float jumpForce;
    public float pickupRadius;
    public float radius;
    public float sprintSpeed;
    public float stamina;
    public float staminaCost;
    public float staminaRecoveryRate;
    public float totalStamina;
    public float walkSpeed;
    public float ySpeed;
    public float fatigueTimer;

    public SerializableDictionary<SerializableDictionary<Item, Destroyable>, SerializableDictionary<int, int>> items;
    public SerializableDictionary<SerializableDictionary<Item, Weapon>, int> weapons;
    public SerializableDictionary<Item, SerializableDictionary<int, int>> materials;
    public PlayerState()
    {
        maxHealthPoints = 100;
        currentHealthPoints = 100;
        maxHungerPoints = 100;
        currentHungerPoints = 100;
        maxHydrationPoints = 100;
        currentHydrationPoints = 100;
        currentSpeed = 1.25f;
        exponentialPenalty = 3;
        jumpForce = 5;
        pickupRadius = 2;
        radius = 3;
        sprintSpeed = 2.5f;
        stamina = 100;
        staminaCost = 10;
        staminaRecoveryRate = 5;
        totalStamina = 100;
        walkSpeed = 1.25f;
        ySpeed = 1.25f;
        fatigueTimer = 4;
        items = new SerializableDictionary<SerializableDictionary<Item, Destroyable>, SerializableDictionary<int, int>>();
        weapons = new SerializableDictionary<SerializableDictionary<Item, Weapon>, int>();
        materials = new SerializableDictionary<Item, SerializableDictionary<int, int>>();
    }
}
