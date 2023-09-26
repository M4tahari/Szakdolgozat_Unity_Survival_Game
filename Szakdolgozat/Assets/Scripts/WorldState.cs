using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[System.Serializable]
public class WorldState
{
    public Vector3 playerPos;
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

    public string worldName;
    public int seed;
    public int randomizationValue;
    public int mapSize;
    public float surfaceLevel;
    public float terrainFrequency;
    public float caveFrequency;
    public float heightMultiplier;
    public bool generateCaves;
    public int heightAddition;
    public int chunkSize;
    public bool alreadyCreated;

    public SerializableDictionary<SerializableDictionary<Vector2, string>, int> blocksPos;
    public WorldState()
    {
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

        randomizationValue = 10000000;
        mapSize = InputTextHandler.mapSize;
        surfaceLevel = InputTextHandler.surfaceLevel;
        terrainFrequency = 0.05f;
        caveFrequency = 0.05f;
        heightMultiplier = 30f;
        heightAddition = InputTextHandler.heightAddition;
        generateCaves = true;
        chunkSize = 20;
        alreadyCreated = false;

        playerPos = new Vector3((mapSize * 0.32f) / 2, surfaceLevel + heightAddition + 1, 0);
        blocksPos = new SerializableDictionary<SerializableDictionary<Vector2, string>, int>();
    }
}
