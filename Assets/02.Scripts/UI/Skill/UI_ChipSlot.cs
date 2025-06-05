using UnityEngine.UI;
public class UI_ChipSlot : UI_InventorySlot
{
    public UI_EquipSkill TargetSkill;
    private Image _slotImage;

    private void Awake()
    { 
        _slotImage = GetComponent<Image>();
    }

    public override void SetItem(Item item)
    {
        base.SetItem(item);

        if (TargetSkill != null)
        {
            TargetSkill.RefreshChipEffects();
        }
    }

    public override void ClearItem()
    {
        base.ClearItem();
        
        if (TargetSkill != null)
        {
            TargetSkill.RefreshChipEffects();
        }
    }
}
