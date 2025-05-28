public class UI_ChipSlot : UI_InventorySlot
{
    public UI_EquipSkill TargetSkill;

    public override void SetItem(Item item)
    {
        base.SetItem(item);
        if (item == null)
        {
            return;
        }
        TargetSkill.SetChipOption(item.Data as ChipDataSO);
    }

    protected override void ClearItem()
    {
        if (Item != null)
        {
            TargetSkill.ClearChipOption(Item.Data as ChipDataSO);
        }
        base.ClearItem();
    }
}
