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
    public void SetChipOption(chipDataSO chipDataSO)
    {
        SkillManager.Instance.EquippedSkills[Index].SkillData.CoolTime *= chipDataSO.ReduceCooldown;
        SkillManager.Instance.EquippedSkills[Index].SkillData.SkillRange *= chipDataSO.SkillRange;

    }

    public void ClearChipOption(chipDataSO chipDataSO)
    {
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
                SetChipOption(chipSlot.Item.Data as chipDataSO );
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
                ClearChipOption(chipSlot.Item.Data as chipDataSO);
            }
        }
    }
}
