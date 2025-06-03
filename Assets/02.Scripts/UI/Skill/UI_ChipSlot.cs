public class UI_ChipSlot : UI_InventorySlot
{
    public UI_EquipSkill TargetSkill;

    public override void SetItem(Item item)
    {
        base.SetItem(item);
        if (TargetSkill != null)
        {
            TargetSkill.RefreshChipEffects();
        }
    }

    protected override void ClearItem()
    {
        base.ClearItem();
        if (TargetSkill != null)
        {
            TargetSkill.RefreshChipEffects();
        }
    }
}
