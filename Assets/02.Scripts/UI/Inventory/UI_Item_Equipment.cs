using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Item_Equipment : UI_Itembase
{
    public ItemEquipment Data;
    
    public override void OnEndDrag(PointerEventData eventData)
    {
        
    }

    

    public override void Init(Item item, GameObject inventorySlot)
    {
        //Item = item;
        base.Init(item, inventorySlot);

        item = Data as Item;
     
        if (item != null &&item.Icon != null)
        {
            GetComponent<Image>().sprite = item.Icon;
        }
        SetItem(inventorySlot);
    }



    public void SetItem(GameObject slot)
    {
        InventorySlot = slot;
        //InventorySlot.GetComponent<InventorySlot>().Item = Item;
    }

    public void RemoveSlotItem()
    {
        if (InventorySlot == null) return;
        InventorySlot.GetComponent<InventorySlot>().Item = null;
    }


}
