public class UI_EquipSkill : UI_Skill
{
    // public Item[] slot;
    public int Index;
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
