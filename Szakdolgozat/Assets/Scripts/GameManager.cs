using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class GameManager : MonoBehaviour
{
    private string fileName;
    private WorldState state;
    private List<Persistance> persistanceObjects;
    public static GameManager instance { get; private set;}

    private WorldFileHandler fileHandler;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Error!");
        }
        instance = this;
    }
    private void Start()
    {
        if(InputTextHandler.worldName != null)
        {
            fileName = InputTextHandler.worldName + ".json";
        }

        else
        {
            fileName = "Uj vilag.json";
        }
        
        this.fileHandler = new WorldFileHandler(Application.persistentDataPath, fileName);
        this.persistanceObjects = FindAllPersistanceObjects();
        LoadGame();
    }
    public void NewGame()
    {
        this.state = new WorldState();
    }
    public void LoadGame()
    {
        this.state = fileHandler.Load();

        if(this.state == null)
        {
            Debug.Log("No data was found, initializing default data.");
            NewGame();
        }

        foreach(Persistance persistance in persistanceObjects)
        {
            persistance.LoadData(state);
        }
    }
    public void SaveGame()
    {
        string worldSave = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(worldSave))
        {
            File.Delete(worldSave);
        }

        foreach (Persistance persistance in persistanceObjects)
        {
            persistance.SaveData(ref state);
        }

        fileHandler.Save(state);
    }
    public List<Persistance> FindAllPersistanceObjects()
    {
        IEnumerable<Persistance> persistanceObjects = FindObjectsOfType<MonoBehaviour>().OfType<Persistance>();
        return new List<Persistance>(persistanceObjects);
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
