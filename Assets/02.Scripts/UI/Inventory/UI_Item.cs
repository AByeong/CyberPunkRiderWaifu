using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Item : MonoBehaviour, IDragHandler, IPointerEnterHandler, IEndDragHandler, IBeginDragHandler
{
    private RectTransform _rectTransform;
    private Canvas _canvas;
    public Item MyItem;
    public GameObject InventorySlot;
    private void Start()
    {
        
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();

      
    }

    public void Init(Item item, GameObject inventorySlot)
    {
        MyItem = item;
        inventorySlot.GetComponent<InventorySlot>().item = item;
        GetComponent<Image>().sprite = item.Icon;
        SetItem(inventorySlot);
    }
    public void SetPosition()
    {
        transform.SetParent(InventorySlot.transform);
        _rectTransform.anchoredPosition = Vector2.zero;
    }
    public void SetItem(GameObject slot)
    {
        InventorySlot = slot;
        InventorySlot.GetComponent<InventorySlot>().item = MyItem;
    }

    public void RemoveSlotItem()
    {
        if(InventorySlot == null) return;
        InventorySlot.GetComponent<InventorySlot>().item = null;
    }
    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Debug.Log($"{eventData.pointerEnter.name} : Enter");
        // Debug.Log($"{eventData.pointerDrag.name} : Drag");
        // if (eventData.pointerEnter?.GetComponent<InventorySlot>() != null)
        // {
        //     InventorySlot = eventData.pointerEnter;
        // }
        // transform.SetParent(InventorySlot.transform);
        // _rectTransform.anchoredPosition = Vector2.zero;
        //
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(_canvas.transform);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
}

