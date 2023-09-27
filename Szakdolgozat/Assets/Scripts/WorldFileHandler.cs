using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class WorldFileHandler
{
    private string filePath = "";
    private string fileName = "";
    public WorldFileHandler(string filePath, string fileName)
    {
        this.filePath = filePath;
        this.fileName = fileName;
    }

    public WorldState Load()
    {
        string fullPath = Path.Combine(filePath, fileName);
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

                loadedState = JsonUtility.FromJson<WorldState>(worldToLoad);
            }

            catch (Exception e)
            {
                Debug.LogError("An error occured when trying to load world data from a file: " + fullPath + "\n" + e);
            }
        }

        return loadedState;
    }
    public void Save(WorldState worldState)
    {
        string fullPath = Path.Combine(filePath, fileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(worldState, true);

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
}
