using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileHandler
{
    private string filePath;
    private string fileName = "";
    private bool encryption = false;
    private readonly string encryptionCodeWord = "jungle";
    public FileHandler(string filePath, string fileName, bool encryption)
    {
        this.filePath = filePath;
        this.fileName = fileName;
        this.encryption = encryption;
    }
    public string GetFileName()
    {
        return this.fileName;
    }
    public WorldState LoadWorld(string worldID)
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
    public void SaveWorld(WorldState worldState, string worldID)
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
    public PlayerState LoadPlayer(string playerID)
    {
        string fullPath = Path.Combine(filePath, playerID, playerID + "_player_data.json");

        PlayerState loadedState = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string playerToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        playerToLoad = reader.ReadToEnd();
                    }
                }

                if (encryption)
                {
                    playerToLoad = EncryptAndDecrypt(playerToLoad);
                }

                loadedState = JsonUtility.FromJson<PlayerState>(playerToLoad);
            }

            catch (Exception e)
            {
                Debug.LogError("An error occured when trying to load player data from a file: " + fullPath + "\n" + e);
            }
        }

        return loadedState;
    }
    public void SavePlayer(PlayerState playerState, string playerID)
    {
        string fullPath = Path.Combine(filePath, playerID, playerID + "_player_data.json"); ;

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(playerState, true);

            if (encryption)
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

        catch (Exception e)
        {
            Debug.LogError("An error occured when trying to save player data into a file: " + fullPath + "\n" + e);
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
                WorldState worldData = LoadWorld(worldName);

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
    public Dictionary<string, PlayerState> LoadSavedPlayer()
    {
        Dictionary<string, PlayerState> savedPlayerData = new Dictionary<string, PlayerState>();

        IEnumerable<DirectoryInfo> playerInfos = new DirectoryInfo(filePath).EnumerateDirectories();

        foreach (DirectoryInfo info in playerInfos)
        {
            string playerId = info.Name;
            string fullPath = Path.Combine(filePath, playerId, playerId + "_player_data.json");

            if (!File.Exists(fullPath))
            {
                Debug.LogError("this directory does not contain any player data, thus its skipped when loading" + playerId);
                continue;
            }

            else
            {
                PlayerState playerData = LoadPlayer(playerId);

                if (playerData != null)
                {
                    savedPlayerData.Add(playerId, playerData);
                }

                else
                {
                    Debug.LogError("Something went wrong when trying to load this player data: " + playerId);
                }
            }
        }

        return savedPlayerData;
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
