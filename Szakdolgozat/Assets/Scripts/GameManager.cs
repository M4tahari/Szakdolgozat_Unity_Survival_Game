using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Header("File storage config")]
    [SerializeField] private string fileName;

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
