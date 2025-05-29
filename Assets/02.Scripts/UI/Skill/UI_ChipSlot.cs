public class UI_ChipSlot : UI_InventorySlot
{
    public UI_EquipSkill TargetSkill;

    public override void SetItem(Item item)
    {
        base.SetItem(item);
        if (item == null || TargetSkill == null)
        {
            return;
        }
        TargetSkill.TrySetChipOption(item.Data as ChipDataSO);
    }

    protected override void ClearItem()
    {
        if (Item != null)
        {
            TargetSkill.TryClearChipOption(Item.Data as ChipDataSO);
        }
        base.ClearItem();
    }
}
