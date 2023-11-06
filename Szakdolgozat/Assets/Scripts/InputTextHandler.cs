using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;
using System.Collections.Generic;

public class InputTextHandler : MonoBehaviour
{
    public static string worldName;
    public static int seed;
    public static int mapSize;
    public static float surfaceLevel;
    public static int heightAddition;
    public static int treeMultiplier;
    public static bool generateCaves = true;
    private int randomizationValue = 10000000;
    public Text nameText;
    public Text seedText;
    public Dropdown sizeOutput;
    public Slider slider;
    public Toggle toggle;
    const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    public void Start()
    {
        ChooseWorldSize(0);
    }
    public void GenerateWorld()
    {
        if(nameText.text == "")
        {
            worldName = "Uj vilag";
        }

        else
        {
            IEnumerable<DirectoryInfo> worldInfos = new DirectoryInfo(Application.persistentDataPath).EnumerateDirectories();

            List<string> worldNames = new List<string>();

            foreach(DirectoryInfo info in worldInfos)
            {
                worldNames.Add(info.Name);
            }

            if(worldNames.Contains(nameText.text) == false)
            {
                worldName = nameText.text;
            }

            else
            {
                var random = new System.Random();
                int length = (int)random.NextSingle(1, 13);
                char[] result = new char[length];

                for(int i = 0; i < length; i++)
                {
                    result[i] = characters[random.Next(characters.Length)];
                }

                worldName = new string(result);

                Debug.LogError("Ilyen nevű világ már létezik!");
            }
        }
        
        if(seedText.text == "" || !(int.Parse(seedText.text) >= -randomizationValue && int.Parse(seedText.text) <= randomizationValue))
        {
            seed = Random.Range(-randomizationValue, randomizationValue);
        }

        else
        {
            seed = int.Parse(seedText.text);
        }

        surfaceLevel = 0.2f;
        heightAddition = 20;
        treeMultiplier = (int) slider.value;
       
        string worldSave = Path.Combine(Application.persistentDataPath, worldName);
        if (File.Exists(worldSave))
        {
            File.Delete(worldSave);
        }

        MainMenu._sceneIndex = 4;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
    }
    public void ChooseWorldSize(int value)
    {
        if (value == 0)
        {
            mapSize = 1000;
        }

        if (value == 1)
        {
            mapSize = 2000;
        }

        if (value == 2)
        {
            mapSize = 4000;
        }
    }
    public void SetGenerateCaves()
    {
        generateCaves = !generateCaves;
    }
}


