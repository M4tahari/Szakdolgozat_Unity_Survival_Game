using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;

public class InputTextHandler : MonoBehaviour
{
    public static string worldName;
    public static int seed;
    public static int mapSize;
    public static float surfaceLevel;
    public static int heightAddition;
    private int randomizationValue = 10000000;
    public Text nameText;
    public Text seedText;
    public Dropdown sizeOutput;

    public void GenerateWorld()
    {
        if(nameText.text == "")
        {
            worldName = "Uj vilag";
        }

        else
        {
            worldName = nameText.text;
        }
        
        if(seedText.text == "" || !(int.Parse(seedText.text) >= -randomizationValue && int.Parse(seedText.text) <= randomizationValue))
        {
            seed = UnityEngine.Random.Range(-randomizationValue, randomizationValue);
        }

        else
        {
            seed = int.Parse(seedText.text);
        }

        surfaceLevel = 0.2f;
        heightAddition = 20;
       
        string worldSave = Path.Combine(Application.persistentDataPath, worldName);
        if (File.Exists(worldSave))
        {
            File.Delete(worldSave);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
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
}


