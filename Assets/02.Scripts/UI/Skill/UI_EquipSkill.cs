using UnityEngine;
public class UI_EquipSkill : UI_Skill
{
    public ChipSlot[] ChipSlots;
    public int Index = -1;
    private void Start()
    {
        if (Index < 0)
            return;

        if (SkillManager.Instance.EquippedSkills[Index] != null)
        {
            SetSkill(SkillManager.Instance.EquippedSkills[Index], true);
        }
    }
    public void SetChipOption(ChipData item)
    {
        SkillManager.Instance.EquippedSkills[Index].SkillData.CoolTime *= item.ReduceCooldown;
        SkillManager.Instance.EquippedSkills[Index].SkillData.SkillRange *= item.SkillRange;

    }

    public void ClearChipOption(ChipData item)
    {
        SkillManager.Instance.EquippedSkills[Index].SkillData.CoolTime /= item.ReduceCooldown;
        SkillManager.Instance.EquippedSkills[Index].SkillData.SkillRange /= item.SkillRange;

    }

    public override void SetSkill(Skill skillToEquip, bool isActive)
    {
        base.SetSkill(skillToEquip, isActive);
        foreach(ChipSlot chipSlot in ChipSlots)
        {
            if (chipSlot.UI_Item != null)
            {
            }
        }
    }

    public override void RemoveSkill()
    {
        base.RemoveSkill();
        foreach(ChipSlot chipSlot in ChipSlots)
        {
            if (chipSlot.UI_Item != null)
            {
            }
        }
    }
}
