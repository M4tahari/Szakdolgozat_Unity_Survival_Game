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
    private List<string> FileNames;
    [SerializeField] private bool useEncryption;
    public WorldState worldState;
    public PlayerState playerState;
    private List<Persistance> persistanceObjects;
    public static GameManager instance { get; private set;}

    private List<FileHandler> FileHandlers;
    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Error!");
        }

        instance = this;
        this.FileNames = new List<string>();
        this.FileHandlers = new List<FileHandler>();
    }
    private void Start()
    {
        IEnumerable<DirectoryInfo> fileInfos = new DirectoryInfo(Application.persistentDataPath).EnumerateDirectories();
        this.persistanceObjects = FindAllPersistanceObjects();

        foreach (DirectoryInfo info in fileInfos)
        {
            if(FileNames.Contains(info.Name) == false)
            {
                FileNames.Add(info.Name);
            }
        }

        if (InputTextHandler.worldName != null && FileNames.Contains(InputTextHandler.worldName) == false)
        {
            FileNames.Add(InputTextHandler.worldName);
        }
 
        foreach (string name in FileNames)
        {
            this.FileHandlers.Add(new FileHandler(Application.persistentDataPath, name, useEncryption));

            if(MainMenu._sceneIndex == 0)
            {
                LoadGame(name);
            }
        }

        if(InputTextHandler.worldName != null)
        {
            LoadGame(InputTextHandler.worldName);
        }

        virtualCamera.m_Lens.OrthographicSize *= SettingsInputHandler.renderDistanceMultiplier;
    }
    public void NewGame()
    {
        this.worldState = new WorldState();
        this.playerState = new PlayerState();
    }
    public void LoadGame(string worldName)
    {
        foreach(FileHandler fileHandler in this.FileHandlers)
        {
            if(fileHandler.GetFileName().Equals(worldName))
            {
                this.worldState = fileHandler.LoadWorld(worldName);
                this.playerState = fileHandler.LoadPlayer(worldName);
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
            persistance.LoadData(this.worldState, this.playerState);
        }
    }
    public void LoadGameIfStateIsNull()
    {
        if (this.worldState == null && this.playerState == null)
        {
            Debug.Log("No data was found.");
            NewGame();
        }
    }
    public void SaveGame(string worldName)
    {
        foreach(FileHandler fileHandler in FileHandlers)
        {
            if(fileHandler.GetFileName().Equals(worldName))
            {
                string worldSave = Path.Combine(Application.persistentDataPath, fileHandler.GetFileName(), fileHandler.GetFileName() + ".json");
                string playerSave = Path.Combine(Application.persistentDataPath, fileHandler.GetFileName(), fileHandler.GetFileName() + ".json");

                if (File.Exists(worldSave))
                {
                    File.Delete(worldSave);
                }

                if(File.Exists(playerSave))
                {
                    File.Delete(playerSave);
                }

                foreach (Persistance persistance in persistanceObjects)
                {
                    persistance.SaveData(ref this.worldState, ref this.playerState);
                }

                fileHandler.SaveWorld(this.worldState, fileHandler.GetFileName());
                fileHandler.SavePlayer(this.playerState, fileHandler.GetFileName());
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

        foreach (FileHandler fileHandler in FileHandlers)
        {
            Dictionary<string, WorldState> loadedWorld = fileHandler.LoadSavedWorld();

            savedWorlds = loadedWorld;
        }

        return savedWorlds;
    }
    public Dictionary<string, PlayerState> GetAllSavedPlayers()
    {
        Dictionary<string, PlayerState> savedPlayers = new Dictionary<string, PlayerState>();

        foreach (FileHandler fileHandler in FileHandlers)
        {
            Dictionary<string, PlayerState> loadedPlayer = fileHandler.LoadSavedPlayer();

            savedPlayers = loadedPlayer;
        }

        return savedPlayers;
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("SliderValue");
    }
}
