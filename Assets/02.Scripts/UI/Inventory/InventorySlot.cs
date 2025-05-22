using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public GameObject ItemPrefab;
    public UI_Item UI_Item;
    public Item item;
    public bool HasItem;
    
    private void Start()
    {
        if (HasItem)
        {
            GameObject newitem = Instantiate(ItemPrefab, transform);
            UI_Item = newitem.GetComponent<UI_Item>();
            UI_Item.Init(item, gameObject);
        }
    }
    public void SetItem(Item newItem)
    {
        item = newItem;
        HasItem = true;

        if (UI_Item == null)
        {
            GameObject newUI = Instantiate(ItemPrefab, transform);
            UI_Item = newUI.GetComponent<UI_Item>();
        }

        UI_Item.Init(newItem, gameObject);
    }
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerDrag.name + " dropped");
        if (eventData.pointerDrag.TryGetComponent(out UI_Item))
        {
            UI_Item.RemoveSlotItem();
            UI_Item.SetItem(gameObject);
            UI_Item.SetPosition();
        }
    }
}
