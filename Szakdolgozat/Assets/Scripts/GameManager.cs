using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using Cinemachine;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    private List<string> fileNames;
    [SerializeField] private bool useEncryption;
    private WorldState state;
    private List<Persistance> persistanceObjects;
    public static GameManager instance { get; private set;}

    private List<WorldFileHandler> fileHandlers;
    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Error!");
        }

        instance = this;
        this.fileNames = new List<string>();
        this.fileHandlers = new List<WorldFileHandler>();
    }
    private void Start()
    {
        IEnumerable<DirectoryInfo> worldInfos = new DirectoryInfo(Application.persistentDataPath).EnumerateDirectories();
        this.persistanceObjects = FindAllPersistanceObjects();

        foreach (DirectoryInfo info in worldInfos)
        {
            if(fileNames.Contains(info.Name) == false)
            {
                fileNames.Add(info.Name);
            }
        }

        if (InputTextHandler.worldName != null && fileNames.Contains(InputTextHandler.worldName) == false)
        {
            fileNames.Add(InputTextHandler.worldName);
        }
 
        foreach (string name in fileNames)
        {
            this.fileHandlers.Add(new WorldFileHandler(Application.persistentDataPath, name, useEncryption));
            LoadGame(name);
        }

        if(InputTextHandler.worldName != null)
        {
            LoadGame(InputTextHandler.worldName);
        }

        virtualCamera.m_Lens.OrthographicSize *= SettingsInputHandler.renderDistanceMultiplier;
    }
    public void NewGame()
    {
        this.state = new WorldState();
    }
    public void LoadGame(string name)
    {
        foreach(WorldFileHandler fileHandler in this.fileHandlers)
        {
            if(fileHandler.GetFileName().Equals(name))
            {
                this.state = fileHandler.Load(name);
                break;
            }

            else
            {
                continue;
            }
        }

        LoadGameIfStateIsNull();

        foreach (Persistance persistance in persistanceObjects)
        {
            persistance.LoadData(this.state);
        }
    }
    public void LoadGameIfStateIsNull()
    {
        if (this.state == null)
        {
            Debug.Log("No data was found.");
            NewGame();
        }
    }
    public void SaveGame(string name)
    {
        foreach(WorldFileHandler fileHandler in fileHandlers)
        {
            if(fileHandler.GetFileName().Equals(name))
            {
                string worldSave = Path.Combine(Application.persistentDataPath, fileHandler.GetFileName(), fileHandler.GetFileName() + ".json");

                if (File.Exists(worldSave))
                {
                    File.Delete(worldSave);
                }

                foreach (Persistance persistance in persistanceObjects)
                {
                    persistance.SaveData(ref this.state);
                }

                fileHandler.Save(this.state, fileHandler.GetFileName());
                break;
            }
        }
    }
    public void SaveOnClick()
    {
        SaveGame(InputTextHandler.worldName);
    }
    public List<Persistance> FindAllPersistanceObjects()
    {
        IEnumerable<Persistance> persistanceObjects = FindObjectsOfType<MonoBehaviour>().OfType<Persistance>();
        return new List<Persistance>(persistanceObjects);
    }
    public Dictionary<string, WorldState> GetAllSavedWorlds()
    {
        Dictionary<string, WorldState> savedWorlds = new Dictionary<string, WorldState>();

        foreach (WorldFileHandler fileHandler in fileHandlers)
        {
            Dictionary<string, WorldState> loadedWorld = fileHandler.LoadSavedWorld();

            savedWorlds = loadedWorld;
        }

        return savedWorlds;
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("SliderValue");
    }
}
