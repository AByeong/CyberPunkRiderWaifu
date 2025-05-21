using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour, IDropHandler
{
    public Image icon;
    public Item equippedItem;

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot originSlot = eventData.pointerDrag?.GetComponent<InventorySlot>();

        if (originSlot != null && originSlot.currentItem != null &&
            originSlot.currentItem.ItmeType == ItemType.Equipment)
        {
            EquipItem(originSlot.currentItem);
            originSlot.ClearSlot();
        }
    }

    public void EquipItem(Item item)
    {
        equippedItem = item;
        icon.sprite = item.Icon;
        icon.enabled = true;
        Debug.Log($"[장착됨] {item.ItemName}");
    }
}