using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class WorldFileHandler
{
    private string filePath = "";
    private string fileName = "";
    private bool encryption = false;
    private readonly string encryptionCodeWord = "jungle";
    public WorldFileHandler(string filePath, string fileName, bool encryption)
    {
        this.filePath = filePath;
        this.fileName = fileName;
        this.encryption = encryption;
    }
    public string GetFileName()
    {
        return this.fileName;
    }
    public WorldState Load(string worldID)
    {
        string fullPath = Path.Combine(filePath, worldID, worldID + ".json");
      
        WorldState loadedState = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string worldToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        worldToLoad = reader.ReadToEnd();
                    }
                }

                if(encryption)
                {
                    worldToLoad = EncryptAndDecrypt(worldToLoad);
                }

                loadedState = JsonUtility.FromJson<WorldState>(worldToLoad);
            }

            catch (Exception e)
            {
                Debug.LogError("An error occured when trying to load world data from a file: " + fullPath + "\n" + e);
            }
        }

        return loadedState;
    }
    public void Save(WorldState worldState, string worldID)
    {
        string fullPath = Path.Combine(filePath, worldID, worldID + ".json"); ;

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(worldState, true);

            if(encryption)
            {
                dataToStore = EncryptAndDecrypt(dataToStore);
            }

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }

        catch(Exception e)
        {
            Debug.LogError("An error occured when trying to save world data into a file: " + fullPath + "\n" + e);
        }
    }
    public Dictionary<string, WorldState> LoadSavedWorld()
    {
        Dictionary<string, WorldState> savedWorldData = new Dictionary<string, WorldState>();
       
        IEnumerable<DirectoryInfo> worldInfos = new DirectoryInfo(filePath).EnumerateDirectories();

        foreach(DirectoryInfo info in worldInfos)
        {
            string worldName = info.Name;
            string fullPath = Path.Combine(filePath, worldName, worldName + ".json");

            if(!File.Exists(fullPath))
            {
                Debug.LogError("this directory does not contain any world data, thus its skipped when loading" + worldName);
                continue;
            }

            else
            {
                WorldState worldData = Load(worldName);

                if (worldData != null)
                {
                    savedWorldData.Add(worldName, worldData);
                }

                else
                {
                    Debug.LogError("Something went wrong when trying to load this world data: " + worldName);
                }                  
            }
        }

        return savedWorldData;
    }
    private string EncryptAndDecrypt(string data)
    {
        string encryptedData = "";

        for(int i = 0; i < data.Length; i++)
        {
            encryptedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }

        return encryptedData;
    }
}
