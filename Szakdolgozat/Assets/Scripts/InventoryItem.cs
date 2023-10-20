using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI")]
    public Image image;
    public Text countText;
    [HideInInspector] public Item item;
    [HideInInspector] public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;
    public Destroyable destroyable;
    [HideInInspector] public bool buildMode = false;
    [HideInInspector] public int clickCount = 0;
    public void Update()
    {
        bool isOverItem = (Input.mousePosition.x >= gameObject.transform.position.x-16 && 
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
    public void InitializeItem(Item newItem)
    {
        item = newItem;
        image.sprite = item.sprite;
        RefreshCount();
    }
    public void RefreshCount()
    {
        countText.text = count.ToString();
        bool isCountMoreThanOne = count > 1;
        countText.gameObject.SetActive(isCountMoreThanOne);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
    }
    public void OnDestroy()
    {
        item.currentAmount = 0;
    }
}
