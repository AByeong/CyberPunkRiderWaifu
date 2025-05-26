using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Item_Etc : UI_Itembase
{
    public ItemEtc Data;
    public override void OnEndDrag(PointerEventData eventData)
    {
        
    }

    

    public override void Init(Item item, GameObject inventorySlot)
    {
        Item = item;
        OriginalSlot = inventorySlot;
        InventorySlot = inventorySlot;
     
        if (item != null &&item.Icon != null)
        {
            GetComponent<Image>().sprite = item.Icon;
        }
        SetItem(inventorySlot);
    }



    public void SetItem(GameObject slot)
    {
        InventorySlot = slot;
        InventorySlot.GetComponent<InventorySlot>().Item = Item;
    }

    public void RemoveSlotItem()
    {
        if (InventorySlot == null) return;
        InventorySlot.GetComponent<InventorySlot>().Item = null;
    }


}