public class EquipmentSlot : InventorySlot
{
    public EquipmentType equipmentType;
    public override void SetData(Item newItem)
    {
        base.SetData(newItem);
    }

    public override void ClearSlot()
    {
        base.ClearSlot();
    }
}
