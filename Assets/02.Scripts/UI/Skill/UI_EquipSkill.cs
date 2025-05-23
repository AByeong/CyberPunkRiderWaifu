using System;
public class UI_EquipSkill : UI_Skill
{
    // public Item[] slot;
    public int Index;
    private void OnEnable()
    {
        if (SkillManager.Instance.EquippedSkills[Index] != null)
        {
            SetSkill(SkillManager.Instance.EquippedSkills[Index], true);
        }
    }
    public void SetChipOption(Item item)
    {
        SkillManager.Instance.EquippedSkills[Index].SkillData.CoolTime *= item.ChipData.ReduceCooldown;
        SkillManager.Instance.EquippedSkill1.SkillData.SkillRange *= item.ChipData.SkillRange;

    }

    public void ClearChipOption(Item item)
    {
        SkillManager.Instance.EquippedSkills[Index].SkillData.CoolTime /= item.ChipData.ReduceCooldown;
        SkillManager.Instance.EquippedSkills[Index].SkillData.SkillRange /= item.ChipData.SkillRange;

    }
}
