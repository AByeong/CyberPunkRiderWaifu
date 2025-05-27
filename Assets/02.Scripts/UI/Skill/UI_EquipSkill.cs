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
    public void SetChipOption(ChipData chipData)
    {
        SkillManager.Instance.EquippedSkills[Index].SkillData.CoolTime *= chipData.ReduceCooldown;
        SkillManager.Instance.EquippedSkills[Index].SkillData.SkillRange *= chipData.SkillRange;

    }

    public void ClearChipOption(ChipData chipData)
    {
        SkillManager.Instance.EquippedSkills[Index].SkillData.CoolTime /= chipData.ReduceCooldown;
        SkillManager.Instance.EquippedSkills[Index].SkillData.SkillRange /= chipData.SkillRange;

    }

    public override void SetSkill(Skill skillToEquip, bool isActive)
    {
        base.SetSkill(skillToEquip, isActive);
        foreach(ChipSlot chipSlot in ChipSlots)
        {
            if (chipSlot.UI_Item != null)
            {
                SetChipOption(chipSlot.ChipData);
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
                ClearChipOption(chipSlot.ChipData);
            }
        }
    }
}
