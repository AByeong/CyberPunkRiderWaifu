using UnityEngine;
public class UI_EquipSkill : UI_Skill
{
    public ChipSlot[] ChipSlots;
    public int Index;
    private void Start()
    {
        if (SkillManager.Instance.EquippedSkills[Index] != null)
        {
            SetSkill(SkillManager.Instance.EquippedSkills[Index], true);
        }
    }
    public void SetChipOption(ItemChip item)
    {
        Debug.Log(item.Data.ReduceCooldown);
        SkillManager.Instance.EquippedSkills[Index].SkillData.CoolTime *= item.Data.ReduceCooldown;
        SkillManager.Instance.EquippedSkills[Index].SkillData.SkillRange *= item.Data.SkillRange;

    }

    public void ClearChipOption(ItemChip item)
    {
        SkillManager.Instance.EquippedSkills[Index].SkillData.CoolTime /= item.Data.ReduceCooldown;
        SkillManager.Instance.EquippedSkills[Index].SkillData.SkillRange /= item.Data.SkillRange;

    }

    public override void SetSkill(Skill skillToEquip, bool isActive)
    {
        base.SetSkill(skillToEquip, isActive);
        foreach(ChipSlot chipSlot in ChipSlots)
        {
            if (chipSlot != null)
            {
                //SetChipOption(chipSlot.Items[1].Item);
            }
        }
    }

    public override void RemoveSkill()
    {
        base.RemoveSkill();
        foreach(ChipSlot chipSlot in ChipSlots)
        {
            // if (chipSlot.UI_Item != null)
            // {
            //     ClearChipOption(chipSlot.UI_Item.MyItem);
            // }
        }
    }
}
