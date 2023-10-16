using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveWorldSlotsMenu : MonoBehaviour
{
    private SaveWorldToSlot[] saveWorldSlots;
    [SerializeField] private Button backButton;
    private Dictionary<string, WorldState>allSavedWorlds;
    public GameObject saveWorldSlot;
    public void Start()
    {
        allSavedWorlds = GameManager.instance.GetAllSavedWorlds();

        foreach(KeyValuePair<string, WorldState> savedWorld in allSavedWorlds)
        {
            GameObject slot = Instantiate(saveWorldSlot);
            slot.transform.SetParent(this.transform.GetChild(0).transform);
            slot.transform.position = new Vector3(slot.transform.parent.position.x, slot.transform.parent.position.y, slot.transform.parent.position.z);
        }
        
        saveWorldSlots = this.GetComponentsInChildren<SaveWorldToSlot>();

        ActivateMenu();
    }
    public void ActivateMenu()
    {
        var worldSlotsAndSavedWorlds = saveWorldSlots.Zip(allSavedWorlds, (sl, st) => new { slot = sl, state = st });

        foreach(var slotAndWorld in worldSlotsAndSavedWorlds)
        {
            slotAndWorld.slot.SetData(slotAndWorld.state.Value);
        }
    }
    public void DisableMenuButtons()
    {
        foreach(SaveWorldToSlot saveWorldToSlot in saveWorldSlots)
        {
            saveWorldToSlot.SetInteractable(false);
        }
        backButton.interactable = false;
    }
}
