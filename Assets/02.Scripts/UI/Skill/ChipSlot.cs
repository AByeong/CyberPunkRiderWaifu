public class ChipSlot : InventorySlot
{
    public UI_EquipSkill TargetSkill;
    public ChipData ChipData;
    public override void SetItem(ItemData newItemData)
    {
        base.SetItem(newItemData);
        ChipData = (ChipData)newItemData.Data;
        TargetSkill.SetChipOption(ChipData);
    }

    public override void ClearSlot()
    {

        TargetSkill.ClearChipOption(ChipData);
        base.ClearSlot();
    }
}
