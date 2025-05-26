public class ChipSlot : InventorySlot
{
    public UI_EquipSkill TargetSkill;
    public ChipData ChipData;
    public override void SetItem(Item newItem)
    {
        base.SetItem(newItem);
        ChipData = (ChipData)newItem.Data;
        TargetSkill.SetChipOption(ChipData);
    }

    public override void ClearSlot()
    {

        TargetSkill.ClearChipOption(ChipData);
        base.ClearSlot();
    }
}
