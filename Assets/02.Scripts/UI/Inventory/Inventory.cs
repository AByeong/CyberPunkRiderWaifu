using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<InventorySlot> _inventorySlots;

    public List<InventorySlot> InventorySlots =>  _inventorySlots;
    
    public int InventorySize => InventorySlots.Count;
    //public UnityAction<InventorySlot> OnIn;


}
