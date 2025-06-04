using System;
using System.Drawing;
using UnityEngine.UI;
using Color = UnityEngine.Color;

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
            if(_slotImage != null)
                _slotImage.color = new Color(_slotImage.color.r, _slotImage.color.g, _slotImage.color.b, 1f);
            TargetSkill.RefreshChipEffects();
        }
    }

    protected override void ClearItem()
    {
        base.ClearItem();
        if (TargetSkill != null)
        {
            if(_slotImage != null)
                _slotImage.color = new Color(_slotImage.color.r, _slotImage.color.g, _slotImage.color.b, 0f);
            TargetSkill.RefreshChipEffects();
        }
    }
}
