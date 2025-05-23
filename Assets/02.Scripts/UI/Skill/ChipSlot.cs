using _02.Scripts.UI.Skill;
using UnityEngine;
public class ChipSlot : InventorySlot
{
    public UI_EquipSkill TargetSkill;
    public override void SetItem(Item newItem)
    {
        base.SetItem(newItem);
        Debug.Log("asda111111111111111111111111111111111111111111111111111111111111111111" + UI_Item.MyItem.ChipData);
        TargetSkill.SetChipOption(UI_Item.MyItem);
    }
}
