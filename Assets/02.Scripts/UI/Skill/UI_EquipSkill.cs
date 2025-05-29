public class UI_EquipSkill : UI_Skill
{
    public UI_ChipSlot[] ChipSlots;
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
    public void TrySetChipOption(ChipDataSO chipDataSO)
    {
        if (SkillManager.Instance.EquippedSkills[Index] == null) return;

        SkillManager.Instance.EquippedSkills[Index].SkillData.CoolTime *= chipDataSO.ReduceCooldown;
        SkillManager.Instance.EquippedSkills[Index].SkillData.SkillRange *= chipDataSO.SkillRange;
    }

    public void TryClearChipOption(ChipDataSO chipDataSO)
    {
        if (SkillManager.Instance.EquippedSkills[Index] == null) return;

        SkillManager.Instance.EquippedSkills[Index].SkillData.CoolTime /= chipDataSO.ReduceCooldown;
        SkillManager.Instance.EquippedSkills[Index].SkillData.SkillRange /= chipDataSO.SkillRange;
    }

    public override void SetSkill(Skill skillToEquip, bool isActive)
    {
        base.SetSkill(skillToEquip, isActive);
        foreach(UI_ChipSlot chipSlot in ChipSlots)
        {
            if (chipSlot.HasItem)
            {
                TrySetChipOption(chipSlot.Item.Data as ChipDataSO);
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
                TryClearChipOption(chipSlot.Item.Data as ChipDataSO);
            }
        }
    }
}
