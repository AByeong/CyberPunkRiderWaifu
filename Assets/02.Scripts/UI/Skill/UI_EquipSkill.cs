using UnityEngine.EventSystems;
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
    public void TrySetChipOption()
    {
        if (SkillManager.Instance.EquippedSkills[Index] == null) return;
        ResetBaseSkill(SkillBase);
        foreach(UI_ChipSlot chipSlot in ChipSlots)
        {
            if (chipSlot.Item != null)
            {
                Skill.SkillData.CoolTime *= (chipSlot.Item.Data as ChipDataSO).ReduceCooldown;
                Skill.SkillData.SkillRange += (chipSlot.Item.Data as ChipDataSO).SkillRange;
            }
        }
    }

    public override void SetSkill(Skill skillToEquip, bool isActive)
    {
        base.SetSkill(skillToEquip, isActive);
        TrySetChipOption();
    }
    
    public override void OnPointerEnter(PointerEventData eventData)
    {
        UI_SkillInspector.Instance.Hovered(Skill);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        UI_SkillInspector.Instance.Exit();
    }
}
