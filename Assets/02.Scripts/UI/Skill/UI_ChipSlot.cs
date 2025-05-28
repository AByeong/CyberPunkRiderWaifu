using UnityEngine;
using UnityEngine.Serialization;

public class UI_ChipSlot : UI_InventorySlot
{
    public UI_EquipSkill TargetSkill;

    public override void SetItem(Item item)
    {
        base.SetItem(item);
        if (item == null) return;
        TargetSkill.SetChipOption(item.Data as ChipDataSO);
    }
}
