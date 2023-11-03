using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : InventoryCountable
{
    public Destroyable destroyable;
    [HideInInspector] public bool buildMode = false;
    [HideInInspector] public bool isOverItem = false;
    [HideInInspector] public int clickCount = 0;
    public void Update()
    {
        isOverItem = (Input.mousePosition.x >= gameObject.transform.position.x-16 && 
        Input.mousePosition.x <= gameObject.transform.position.x + 16) &&
        (Input.mousePosition.y >= gameObject.transform.position.y-16 && 
        Input.mousePosition.y <= gameObject.transform.position.y + 16);

        if (buildMode == true)
        {
            transform.position = Input.mousePosition;
        }

        if (isOverItem)
        {
            if (Input.GetMouseButtonDown(1))
            {
                buildMode = true;
                clickCount++;
            }

            if (Input.GetMouseButtonDown(1) && clickCount > 1)
            {
                buildMode = false;
                clickCount = 0;
                transform.position = transform.parent.position;
            }

            if (Input.GetMouseButtonDown(0) && clickCount > 0)
            {
                destroyable.inventoryManager.PlaceItem(this);
            }
        }
    }
}
