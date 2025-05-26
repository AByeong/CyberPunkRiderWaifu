public class EquipmentSlot : InventorySlot
{
    public  EquipmentType equipmentType;
    public override void SetItem(Item newItem)
    {
        base.SetItem(newItem);
    }

    public override void ClearSlot()
    {
        base.ClearSlot();
    }
}
