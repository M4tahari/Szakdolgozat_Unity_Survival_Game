using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System;
using Unity.Mathematics;

public class GenerateMap : MonoBehaviour, Persistance
{
    [Header("Blokkok")]
    public GameObject jungleGroundBlock;
    public GameObject jungleDirtBlock;
    public GameObject termitePlainsGroundBlock;
    public GameObject termitePlainsSandBlock;
    public GameObject wetlandsGroundBlock;
    public GameObject wetlandsMudBlock;
    public GameObject jungleLeaves;
    public GameObject jungleTreeLogBlock;
    public GameObject wetlandsLeaves;
    public GameObject wetlandsTreeLogBlock;
    public GameObject termiteCastleWallBlock;

    [Header("Palya adatok")]
    public string worldName;
    public int mapSize;
    public float surfaceLevel;
    public float terrainFrequency;
    public float caveFrequency;
    public float heightMultiplier;
    public int heightAddition;
    public int seed;
    public bool generateCaves;
    public Texture2D noiseSample;
    public int randomizationValue;
    public int chunkSize;

    [Header("Fa adatok")]
    public float treeFrequency = 0.05f;
    public int treeMultiplier;
    public int minTreeHeight = 8;
    public int maxTreeHeight = 30;
    public int minTreeWidth = 1;
    public int maxTreeWidth = 2;
    public int minLeavesHeight = 4;
    public int maxLeavesHeight = 15;
    public int minLeavesWidth = 6;
    public int maxLeavesWidth = 20;

    [Header("Termeszvar adatok")]
    public float castleFrequency = 0.03f;
    public int castleMultiplier = 10;
    public int minCastleHeight = 2;
    public int maxCastleHeight = 5;

    public Player player;

    private GameObject[] mapChunks;
    int chunkNum = 0;
    private SerializableDictionary<SerializableDictionary<Vector2, string>, int> blocks = new SerializableDictionary<SerializableDictionary<Vector2, string>, int>();
    private List<Vector2> blockPositions = new List<Vector2>();
    private int blockCount = 0;
    private bool alreadyCreated = false;
    private void Start()
    {
        if(alreadyCreated == false)
        {
            if(InputTextHandler.worldName != null)
            {
                worldName = InputTextHandler.worldName;
            }

            else
            {
                worldName = "Uj vilag";
            }

            if(InputTextHandler.mapSize > 0)
            {
                mapSize = InputTextHandler.mapSize;
            }

            else
            {
                mapSize = 1000;
            }

            if (InputTextHandler.treeMultiplier >= 0 && InputTextHandler.treeMultiplier <= 10)
            {
                treeMultiplier = InputTextHandler.treeMultiplier;
            }

            else
            {
                treeMultiplier = 5;
            }

            seed = InputTextHandler.seed;
            surfaceLevel = InputTextHandler.surfaceLevel;
            heightAddition = InputTextHandler.heightAddition;
            generateCaves = InputTextHandler.generateCaves;
            GenerateNoiseSample();
            GenerateChunks();
            GenerateBiomes();
            GenerateTerrain();
        }

        else
        {
            GenerateChunks();
            foreach(KeyValuePair<SerializableDictionary<Vector2, string>, int> blockPos in blocks)
            {
                foreach (var a in blockPos.Key)
                {
                    if (a.Value.Contains("JungleFloorBlock(Clone)"))
                    {
                        PlaceLoadedBlock(jungleGroundBlock, (int)a.Key.x, (int)a.Key.y, blockPos.Value);
                    }

                    else if (a.Value.Contains("DirtBlock(Clone)"))
                    {
                        PlaceLoadedBlock(jungleDirtBlock, (int)a.Key.x, (int)a.Key.y, blockPos.Value);
                    }

                    else if (a.Value.Contains("JungleTreeLog(Clone)"))
                    {
                        PlaceLoadedBlock(jungleTreeLogBlock, (int)a.Key.x, (int)a.Key.y, blockPos.Value);
                    }

                    else if (a.Value.Contains("JungleLeaves(Clone)"))
                    {
                        PlaceLoadedBlock(jungleLeaves, (int)a.Key.x, (int)a.Key.y, blockPos.Value);
                    }

                    else if (a.Value.Contains("TermitePlainsFloorBlock(Clone)"))
                    {
                        PlaceLoadedBlock(termitePlainsGroundBlock, (int)a.Key.x, (int)a.Key.y, blockPos.Value);
                    }

                    else if (a.Value.Contains("SandBlock(Clone)"))
                    {
                        PlaceLoadedBlock(termitePlainsSandBlock, (int)a.Key.x, (int)a.Key.y, blockPos.Value);
                    }

                    else if (a.Value.Contains("TermiteCastleWallBlock(Clone)"))
                    {
                        PlaceLoadedBlock(termiteCastleWallBlock, (int)a.Key.x, (int)a.Key.y, blockPos.Value);
                    }

                    if (a.Value.Contains("WetlandsFloorBlock(Clone)"))
                    {
                        PlaceLoadedBlock(wetlandsGroundBlock, (int)a.Key.x, (int)a.Key.y, blockPos.Value);
                    }

                    else if (a.Value.Contains("MudBlock(Clone)"))
                    {
                        PlaceLoadedBlock(wetlandsMudBlock, (int)a.Key.x, (int)a.Key.y, blockPos.Value);
                    }

                    else if (a.Value.Contains("WetlandsTreeLog(Clone)"))
                    {
                        PlaceLoadedBlock(wetlandsTreeLogBlock, (int)a.Key.x, (int)a.Key.y, blockPos.Value);
                    }

                    else if (a.Value.Contains("WetlandsLeaves(Clone)"))
                    {
                        PlaceLoadedBlock(wetlandsLeaves, (int)a.Key.x, (int)a.Key.y, blockPos.Value);
                    }
                }
            }
        }
    }
    public void FixedUpdate()
    {
        LoadChunk();
    }
    public void LoadData(WorldState state)
    {
        worldName = state.worldName;
        seed = state.seed;
        randomizationValue = state.randomizationValue;
        mapSize = state.mapSize;
        surfaceLevel = state.surfaceLevel;
        terrainFrequency = state.terrainFrequency;
        caveFrequency = state.caveFrequency;
        heightMultiplier= state.heightMultiplier;
        generateCaves= state.generateCaves;
        heightAddition = state.heightAddition;
        chunkSize = state.chunkSize;
        blocks = state.blocksPos;
        alreadyCreated = state.alreadyCreated;
    }
    public void SaveData(ref WorldState state)
    {
        state.worldName = worldName;
        state.seed = seed;
        state.randomizationValue = randomizationValue;
        state.mapSize = mapSize;
        state.surfaceLevel = surfaceLevel;
        state.terrainFrequency = terrainFrequency;
        state.caveFrequency = caveFrequency;
        state.heightMultiplier = heightMultiplier;
        state.heightAddition = heightAddition;
        state.chunkSize = chunkSize;
        SaveBlocks();
        state.blocksPos = blocks;
        alreadyCreated = true;
        state.alreadyCreated = alreadyCreated;
    }
    public void GenerateTerrain()
    {
        for(int i = 0; i < mapSize; i++)
        {
            float height = Mathf.PerlinNoise((i + seed) * terrainFrequency, seed * terrainFrequency) * heightMultiplier + heightAddition;
            
            for(int j = 0; j < height; j++)
            {
                float chunkCoord = (Mathf.Round(i / chunkSize) * chunkSize);
                chunkCoord /= chunkSize;
                GameObject currentChunk = mapChunks[(int)chunkCoord];

                if (j < height - 1)
                {
                    if (generateCaves)
                    {
                        if (noiseSample.GetPixel(i, j).r > surfaceLevel)
                        {
                            if(currentChunk.tag == "TropicalJungle")
                            {
                                PlaceBlock(jungleDirtBlock, i, j);
                            }

                            else if(currentChunk.tag == "TermitePlains")
                            {
                                PlaceBlock(termitePlainsSandBlock, i, j);
                            }

                            else if(currentChunk.tag == "Wetlands")
                            {
                                PlaceBlock(wetlandsMudBlock, i, j);
                            }
                        }
                    }

                    else
                    {
                        if (currentChunk.tag == "TropicalJungle")
                        {
                            PlaceBlock(jungleDirtBlock, i, j);
                        }

                        else if (currentChunk.tag == "TermitePlains")
                        {
                            PlaceBlock(termitePlainsSandBlock, i, j);
                        }

                        else if (currentChunk.tag == "Wetlands")
                        {
                            PlaceBlock(wetlandsMudBlock, i, j);
                        }
                    }
                }

                else
                {
                    if(generateCaves)
                    {
                        if (noiseSample.GetPixel(i, j).r > surfaceLevel)
                        {
                            if (currentChunk.tag == "TropicalJungle")
                            {
                                PlaceBlock(jungleGroundBlock, i, j);
                            }

                            else if (currentChunk.tag == "TermitePlains")
                            {
                                PlaceBlock(termitePlainsGroundBlock, i, j);
                            }

                            else if (currentChunk.tag == "Wetlands")
                            {
                                PlaceBlock(wetlandsGroundBlock, i, j);
                            }
                        }
                    }

                    else
                    {
                        if (currentChunk.tag == "TropicalJungle")
                        {
                            PlaceBlock(jungleGroundBlock, i, j);
                        }

                        else if (currentChunk.tag == "TermitePlains")
                        {
                            PlaceBlock(termitePlainsGroundBlock, i, j);
                        }

                        else if (currentChunk.tag == "Wetlands")
                        {
                            PlaceBlock(wetlandsGroundBlock, i, j);
                        }
                    }

                    if (j >= height-1 && i > 10 && i < mapSize-10)
                    {
                        if(currentChunk.tag == "TropicalJungle")
                        {
                            if (blockPositions.Contains(new Vector2(i * 0.32f, j * 0.32f)))
                            {
                                GenerateTree(jungleTreeLogBlock, jungleLeaves, i, j + 1);
                            }
                        }

                        else if(currentChunk.tag == "TermitePlains")
                        {
                            if (blockPositions.Contains(new Vector2(i * 0.32f, j * 0.32f)))
                            {
                                GenerateTermiteCastles(termiteCastleWallBlock, i, j + 1);
                            }
                        }

                        else if(currentChunk.tag == "Wetlands")
                        {
                            if (blockPositions.Contains(new Vector2(i * 0.32f, j * 0.32f)))
                            {
                                GenerateTree(wetlandsTreeLogBlock, wetlandsLeaves, i, j + 1);
                            }
                        }
                    }
                }
            }
        }
    }
    public void GenerateNoiseSample()
    {
        noiseSample = new Texture2D(mapSize, mapSize);

        for (int i = 0; i < noiseSample.width; i++)
        {
            for (int j = 0; j < noiseSample.height; j++)
            {
                float value = Mathf.PerlinNoise((i + seed) * caveFrequency, (j + seed) * caveFrequency);
                noiseSample.SetPixel(i, j, new Color(value, value, value));
            }
        }

        noiseSample.Apply();
    }
    public void GenerateChunks()
    {
        chunkNum = mapSize / chunkSize;
        mapChunks = new GameObject[chunkNum];

        for(int i = 0; i < chunkNum; i++)
        {
            GameObject chunk = new GameObject();
            chunk.name = i.ToString();
            chunk.transform.parent = this.transform;
            mapChunks[i] = chunk;
        }
    }
    public void GenerateBiomes()
    {
        var random = new System.Random();
        double termitePlainsBiomeValue = random.NextDouble();
        int termitePlainsBiomeSize = (int)random.NextSingle(4, mapChunks.Length / 8);
        int wetlandsBiomeSize = (int)random.NextSingle(4, mapChunks.Length / 8);

        for (int i = 0;i < mapChunks.Length;i++)
        {
            if (i <= 3 || (i >= (mapChunks.Length / 2) - 4 && i <= (mapChunks.Length / 2) + 2) || i >= mapChunks.Length - 4)
            {
                mapChunks[i].tag = "TropicalJungle";
            }

            if(termitePlainsBiomeValue <= 0.5)
            {
                if((i >= (mapChunks.Length / 4) - termitePlainsBiomeSize && i <= (mapChunks.Length / 4) + termitePlainsBiomeSize))
                {
                    mapChunks[i].tag = "TermitePlains";
                }

                else if ((i >= (3 * mapChunks.Length / 4) - wetlandsBiomeSize && i <= (3 * mapChunks.Length / 4) + wetlandsBiomeSize))
                {
                    mapChunks[i].tag = "Wetlands";
                }

                else
                {
                    mapChunks[i].tag = "TropicalJungle";
                }
            }
            
            else if(termitePlainsBiomeValue > 0.5)
            {
                if ((i >= (mapChunks.Length / 4) - wetlandsBiomeSize && i <= (mapChunks.Length / 4) + wetlandsBiomeSize))
                {
                    mapChunks[i].tag = "Wetlands";
                }

                else if ((i >= (3 * mapChunks.Length / 4) - termitePlainsBiomeSize && i <= ( 3 * mapChunks.Length / 4) + termitePlainsBiomeSize))
                {
                    mapChunks[i].tag = "TermitePlains";
                }

                else
                {
                    mapChunks[i].tag = "TropicalJungle";
                }
            }
        }
    }
    public void LoadChunk()
    {
       for(int i = 0;i < mapChunks.Length;i++)
        {
            float distance = Mathf.Abs(Vector3.Distance(player.transform.position, mapChunks[i].transform.GetChild(0).position) / 0.32f);
            
            if(distance > (Player.visibleBlocksRadius * SettingsInputHandler.renderDistanceMultiplier) + 40.0f)
            {
                mapChunks[i].SetActive(false);
            }

            else
            {
                mapChunks[i].SetActive(true);
            }
        }
    }
    public void PlaceBlock(GameObject spawnedBlock, int x, int y)
    {
        GameObject block = Instantiate(spawnedBlock);
        float chunkCoord = (Mathf.Round(x / chunkSize) * chunkSize);
        chunkCoord /= chunkSize;
        block.transform.parent = mapChunks[(int)chunkCoord].transform;
        block.transform.position = new Vector2(x * 0.32f , y * 0.32f);
        blockCount++;
        if (blocks.Count <= blockCount)
        {
            blockPositions.Add(block.transform.position);
        }
    }
    public void PlaceLoadedBlock(GameObject spawnedBlock, int x, int y, int chunk)
    {
        GameObject block = Instantiate(spawnedBlock);
        block.transform.parent = mapChunks[chunk].transform;
        block.transform.position = new Vector2(x * 0.32f, y * 0.32f);

        if (blocks.Count <= blockCount)
        {
            blockPositions.Add(block.transform.position);
        }
    }
    public void GenerateTree(GameObject log, GameObject leaf, int x, int y)
    {
        int treeSeed = seed + (int)(x * 2) + (int)(y / 2);
        var random = new System.Random(treeSeed);
        var random2 = new System.Random(treeSeed);
        float treeChance = random2.NextSingle(0,treeMultiplier * 2);
        float isTreeOn = Mathf.PerlinNoise((x + seed) * treeFrequency, seed * treeFrequency) * treeMultiplier;

        if (isTreeOn >= (treeMultiplier / 10) && isTreeOn < (treeMultiplier / 2) && treeChance < isTreeOn)
        {
            SerializableDictionary<Vector2, string> key = new SerializableDictionary<Vector2, string>();
            key.Add(new Vector2(x, y), log.name + "(Clone)");
            if(!blocks.ContainsKey(key))
            {
                float treeHeight = random.NextSingle(minTreeHeight, maxTreeHeight);
                float treeWidth = random.NextSingle(minTreeWidth, maxTreeWidth);
                float leavesHeight = random.NextSingle(minLeavesHeight, maxLeavesHeight);
                float leavesWidth = random.NextSingle(minLeavesWidth, maxLeavesWidth);

                for (int i = 0; i < treeHeight; i++)
                {
                    for (int j = 0; j < treeWidth; j++)
                    {
                        PlaceBlock(log, x + j, y + i);
                    }
                }

                for (float k = treeHeight; k < treeHeight + leavesHeight; k++)
                {
                    for (float l = -leavesWidth / 2 - treeWidth; l < leavesWidth; l++)
                    {
                        if ((k > treeHeight + (leavesHeight / 6) && k < treeHeight + leavesHeight - 1 - (leavesHeight / 6)) ||
                            (l > -leavesWidth / 2 - treeWidth + (leavesWidth / 6) && l < leavesWidth - 1 - (leavesWidth / 6)))
                        {
                            PlaceBlock(leaf, x + (int)l, y + (int)k);
                        }
                    }
                }
            }
        }
    }
    public void GenerateTermiteCastles(GameObject termiteWall, int x, int y)
    {
        int castleSeed = seed + (int)(x * 2) + (int)(y / 2);
        var random = new System.Random(castleSeed);
        var random2 = new System.Random(castleSeed);
        float castleChance = random2.NextSingle(0, castleMultiplier * 2);
        float isCastleOn = Mathf.PerlinNoise((x + seed) * castleFrequency, seed * castleFrequency) * castleMultiplier;

        if (isCastleOn >= (castleMultiplier / 10) && isCastleOn < (castleMultiplier / 2) && castleChance < isCastleOn)
        {
            SerializableDictionary<Vector2, string> key = new SerializableDictionary<Vector2, string>();
            key.Add(new Vector2(x, y), termiteWall.name + "(Clone)");

            if (!blocks.ContainsKey(key))
            {
                float castleHeight = random.NextSingle(minCastleHeight, maxCastleHeight);
                float castleLength = (int) (castleHeight * 2) - 1;
                int castleMid = (int) castleHeight / 2;
                int castleCounter = 0;

                for(int i = 0; i < castleHeight; i++)
                {
                    for(int j = castleMid - castleCounter; j <= castleMid + castleCounter; j++)
                    {
                        if(j >= castleLength)
                        {
                            break;
                        }

                        else if(j < 0)
                        {
                            continue;
                        }

                        else
                        {
                            PlaceBlock(termiteWall, x + i, y + j);
                        }
                    }

                    castleCounter++;
                }
                
            }

        }
    }
    public void SaveBlocks()
    {
        blocks.Clear();
        if(blocks.Count <= blockCount)
        {
            foreach (Transform chunk in this.transform)
            {
                foreach (Transform block in chunk.transform)
                {
                    SerializableDictionary<Vector2, string> data = new SerializableDictionary<Vector2, string>();
                    data.Add(new Vector2((float)Math.Round(block.transform.position.x / 0.32f, MidpointRounding.ToEven), 
                        (float)Math.Round(block.transform.position.y / 0.32f, MidpointRounding.ToEven)), block.name);
                    blocks.Add(data, int.Parse(chunk.name));
                }
            }

            foreach(Transform placedBlock in this.transform)
            {
                if(placedBlock.name.Any(x => char.IsLetter(x)))
                {
                    SerializableDictionary<Vector2, string> data = new SerializableDictionary<Vector2, string>();
                    data.Add(new Vector2((float)Math.Round(placedBlock.transform.position.x / 0.32f, MidpointRounding.ToEven),
                        (float)Math.Round(placedBlock.transform.position.y / 0.32f, MidpointRounding.ToEven)), placedBlock.name);
                    float chunkCoord = (Mathf.Round((float)Math.Round(placedBlock.transform.position.x / 0.32f, MidpointRounding.ToEven) / chunkSize) * chunkSize);
                    chunkCoord /= chunkSize;
                    blocks.Add(data, (int)chunkCoord);
                }
            }
        }
    }
}
