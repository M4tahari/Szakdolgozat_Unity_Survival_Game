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
    public int randomizationValue = 1000000;
    public Text nameText;
    public Text seedText;
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
       
        string worldSave = Path.Combine(Application.persistentDataPath, worldName);
        if (File.Exists(worldSave))
        {
            File.Delete(worldSave);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
