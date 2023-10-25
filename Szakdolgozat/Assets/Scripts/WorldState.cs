using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[System.Serializable]
public class WorldState
{
    public Vector3 playerPos;
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
    public SerializableDictionary<SerializableDictionary<float, float>, SerializableDictionary<float, float>> termiteCastlesPos;
    public WorldState()
    {
        randomizationValue = 10000000;
        mapSize = InputTextHandler.mapSize;
        surfaceLevel = InputTextHandler.surfaceLevel;
        terrainFrequency = 0.05f;
        caveFrequency = 0.05f;
        heightMultiplier = 30f;
        heightAddition = InputTextHandler.heightAddition;
        generateCaves = InputTextHandler.generateCaves;
        chunkSize = 20;
        alreadyCreated = false;

        playerPos = new Vector3((mapSize * 0.32f) / 2, surfaceLevel + heightAddition + 1, 0);
        blocksPos = new SerializableDictionary<SerializableDictionary<Vector2, string>, int>();
        termiteCastlesPos = new SerializableDictionary<SerializableDictionary<float, float>, SerializableDictionary<float, float>>();
    }
}
