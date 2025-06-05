using UnityEngine;
using UnityEngine.UI;
public class UI_ChipSlot : UI_InventorySlot
{
    public UI_EquipSkill TargetSkill;
    private Image _slotImage;
    public Sprite BaseIcon;
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

        if (IconImageUI != null)
        {
            IconImageUI.sprite = BaseIcon;
            Color _color = IconImageUI.color;
            _color.a = 1;
            IconImageUI.color = _color;    
        }
    }
}
