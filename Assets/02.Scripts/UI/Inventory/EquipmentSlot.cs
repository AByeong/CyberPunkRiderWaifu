using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour, IDropHandler
{
    public Image Icon;
    public Item EquippedItem;

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot originSlot = eventData.pointerDrag?.GetComponent<InventorySlot>();

    }

    public void EquipItem(Item item)
    {
        //equippedItem = item;
      //  icon.sprite = item.Icon;
      //  icon.enabled = true;
        Debug.Log($"[장착됨] {item.ItemName}");
    }
}