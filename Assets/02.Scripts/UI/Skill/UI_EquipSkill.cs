public class UI_EquipSkill : UI_Skill
{
    public UI_ChipSlot[] ChipSlots;
    public int Index;
    private void Start()
    {
        if (SkillManager.Instance.EquippedSkills[Index] != null)
        {
            SetSkill(SkillManager.Instance.EquippedSkills[Index], true);
        }
    }
    public void SetChipOption(ChipDataSO chipDataSo)
    {
        SkillManager.Instance.EquippedSkills[Index].SkillData.CoolTime *= chipDataSo.ReduceCooldown;
        SkillManager.Instance.EquippedSkills[Index].SkillData.SkillRange *= chipDataSo.SkillRange;

    }

    public void ClearChipOption(ChipDataSO chipDataSo)
    {
        SkillManager.Instance.EquippedSkills[Index].SkillData.CoolTime /= chipDataSo.ReduceCooldown;
        SkillManager.Instance.EquippedSkills[Index].SkillData.SkillRange /= chipDataSo.SkillRange;

    }

    public override void SetSkill(Skill skillToEquip, bool isActive)
    {
        base.SetSkill(skillToEquip, isActive);
        foreach(UI_ChipSlot chipSlot in ChipSlots)
        {
            if (chipSlot.HasItem)
            {
                SetChipOption(chipSlot.Item.Data as ChipDataSO );
            }
        }
    }

    public override void RemoveSkill()
    {
        base.RemoveSkill();
        
        foreach(UI_ChipSlot chipSlot in ChipSlots)
        {
            if (chipSlot.HasItem)
            {
                ClearChipOption(chipSlot.Item.Data as ChipDataSO);
            }
        }
    }
}
