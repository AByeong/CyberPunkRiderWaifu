using UnityEngine;
public class ChipSlot : InventorySlot
{
    public UI_EquipSkill TargetSkill;
    // public ItemChip chip;
    private ChipData _chipData;
    // Item을 들고있음 근데 여기선 ItemChip이여야함
    public override void SetData(Item newItem)
    {
        base.SetData(newItem);
        _chipData = (ChipData)Item.Data;
        Debug.Log(((ChipData)Item.Data).ReduceCooldown);
        if (TargetSkill != null)
        {
            TargetSkill.SetChipOption(_chipData);
        }
        // 반환하는 기능 넣어야함
    }

    public override void ClearSlot()
    {
        TargetSkill.ClearChipOption(_chipData);
        base.ClearSlot();
    }
}
