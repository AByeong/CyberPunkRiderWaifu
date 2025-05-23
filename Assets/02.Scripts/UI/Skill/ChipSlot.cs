using _02.Scripts.UI.Skill;
public class ChipSlot : InventorySlot
{
    public UI_EquipSkill TargetSkill;
    public override void SetItem(Item newItem)
    {
        base.SetItem(newItem);
        TargetSkill.SetChipOption(UI_Item.MyItem);
    }

    public override void ClearSlot()
    {

        TargetSkill.ClearChipOption(UI_Item.MyItem);
        base.ClearSlot();
    }
}
