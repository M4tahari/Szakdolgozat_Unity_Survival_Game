using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCountable : InventoryEntity
{
    public Text countText;
    public int count = 1;
    public void RefreshCount()
    {
        countText.text = count.ToString();

        if(count > 1)
        {
            countText.gameObject.SetActive(true);
        }

        else if(count <= 1)
        {
            countText.gameObject.SetActive(false);
        }
    }
}
