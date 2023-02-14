using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class GenerateMap : MonoBehaviour, Persistance
{
    [Header("Blokkok")]
    public GameObject groundBlock;
    public GameObject dirtBlock;
    public GameObject leaves;
    public GameObject sandBlock;
    public GameObject treeLogBlock;

    [Header("Palya adatok")]
    public int mapSize = 100;
    public float surfaceLevel = 0.2f;
    public float terrainFrequency = 0.05f;
    public float caveFrequency = 0.05f;
    public float heightMultiplier = 30f;
    public int heightAddition = 20;
    private float seed;
    public bool generateCaves = true;
    public Texture2D noiseSample;
    public int randomizationValue = 10000;
    public int chunkSize = 16;

    [Header("Fa adatok")]
    public int treeSpawnChance = 10;
    public int minTreeHeight = 8;
    public int maxTreeHeight = 30;
    public int minTreeWidth = 1;
    public int maxTreeWidth = 3;
    public int minLeavesHeight = 4;
    public int maxLeavesHeight = 15;
    public int minLeavesWidth = 6;
    public int maxLeavesWidth = 20;

    private GameObject[] mapChunks;
    private SerializableDictionary<SerializableDictionary<Vector2, string>, int> blocks = new SerializableDictionary<SerializableDictionary<Vector2, string>, int>();
    private List<Vector2> blockPositions = new List<Vector2>();
    private int blockCount = 0;
    private bool alreadyCreated = false;
    private void Start()
    {
        if(alreadyCreated == false)
        {
            seed = UnityEngine.Random.Range(-randomizationValue, randomizationValue);
            GenerateNoiseSample();
            GenerateChunks();
            GenerateTerrain();
        }

        else
        {
            GenerateChunks();
            foreach(KeyValuePair<SerializableDictionary<Vector2, string>, int> blockPos in blocks)
            {
                foreach (var a in blockPos.Key)
                { 
                    if(a.Value.Contains("JungleTreeLog(Clone)"))
                    {
                        PlaceLoadedBlock(treeLogBlock, (int)a.Key.x, (int)a.Key.y, blockPos.Value);
                    }

                    else if(a.Value.Contains("JungleFloorBlock(Clone)"))
                    {
                        PlaceLoadedBlock(groundBlock, (int)a.Key.x, (int)a.Key.y, blockPos.Value);
                    }

                    else if(a.Value.Contains("SandBlock(Clone)"))
                    {
                        PlaceLoadedBlock(sandBlock, (int)a.Key.x, (int)a.Key.y, blockPos.Value);
                    }

                    else if(a.Value.Contains("DirtBlock(Clone)"))
                    {
                        PlaceLoadedBlock(dirtBlock, (int)a.Key.x, (int)a.Key.y, blockPos.Value);
                    }

                    else if(a.Value.Contains("JungleLeaves(Clone)"))
                    {
                        PlaceLoadedBlock(leaves, (int)a.Key.x, (int)a.Key.y, blockPos.Value);
                    }
                }
            }
        }
    }
    public void LoadData(WorldState state)
    {
        mapSize = state.mapSize;
        surfaceLevel = state.surfaceLevel;
        heightAddition = state.heightAddition;
        blocks = state.blocksPos;
        alreadyCreated = state.alreadyCreated;
    }
    public void SaveData(ref WorldState state)
    {
        state.mapSize = mapSize;
        state.surfaceLevel = surfaceLevel;
        state.heightAddition = heightAddition;
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
                if(j < height - 1)
                {
                    if(generateCaves)
                    {
                        if (noiseSample.GetPixel(i, j).r > surfaceLevel)
                        {
                            PlaceBlock(dirtBlock, i, j);
                        }
                    }

                    else
                    {
                        PlaceBlock(dirtBlock, i, j);
                    }
                }

                else
                {
                    if(generateCaves)
                    {
                        if (noiseSample.GetPixel(i, j).r > surfaceLevel)
                        {
                            PlaceBlock(groundBlock, i, j);
                        }
                    }

                    else
                    {
                        PlaceBlock(groundBlock, i, j);
                    }

                    if (j >= height-1 && i > 10 && i < mapSize-10)
                    {
                      if (blockPositions.Contains(new Vector2(i * 0.32f, j * 0.32f)))
                      {
                        GenerateTree(treeLogBlock, leaves, i, j + 1);
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
        int chunkNum = mapSize / chunkSize;
        mapChunks = new GameObject[chunkNum];

        for(int i = 0; i < chunkNum; i++)
        {
            GameObject chunk = new GameObject();
            chunk.name = i.ToString();
            chunk.transform.parent = this.transform;
            mapChunks[i] = chunk;
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
        int isTreeOn = UnityEngine.Random.Range(0, treeSpawnChance);

        if (isTreeOn == 1)
        {
            int treeHeight = UnityEngine.Random.Range(minTreeHeight, maxTreeHeight);
            int treeWidth = UnityEngine.Random.Range(minTreeWidth, maxTreeWidth);
            int leavesHeight = UnityEngine.Random.Range(minLeavesHeight, maxLeavesHeight);
            int leavesWidth = UnityEngine.Random.Range(minLeavesWidth, maxLeavesWidth);

            for (int i = 0; i < treeHeight; i++)
            {
                for(int j = 0; j < treeWidth;j++)
                {
                    PlaceBlock(log, x + j, y + i);
                }
            }

            for (int k = treeHeight; k < treeHeight + leavesHeight; k++)
            {
                for (int l = -leavesWidth / 2 -treeWidth; l < leavesWidth; l++)
                {
                    if((k > treeHeight + (leavesHeight / 6) && k < treeHeight + leavesHeight -1 - (leavesHeight / 6)) || 
                        (l > -leavesWidth / 2 - treeWidth + (leavesWidth / 6) && l < leavesWidth - 1 - (leavesWidth / 6)))
                    {
                        PlaceBlock(leaf, x + l, y + k);
                    }
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
