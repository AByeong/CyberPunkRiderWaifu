using _02.Scripts.UI.Skill;
public class ChipSlot : InventorySlot
{
    public UI_EquipSkill TargetSkill;
    public override void SetItem(Item newItem)
    {
        base.SetItem(newItem);

    }
}
