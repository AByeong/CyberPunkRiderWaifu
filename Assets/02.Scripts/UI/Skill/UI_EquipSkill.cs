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

    public void RefreshChipEffects()
    {
        if (SkillBase == null)
        {
            return;
        }

        // 1. 스킬을 초기 상태로 리셋
        ResetBaseSkill(SkillBase);

        // 2. 모든 장착된 칩의 효과를 누적 적용
        foreach (UI_ChipSlot chipSlot in ChipSlots)
        {
            if (chipSlot.Item != null && chipSlot.Item.Data is ChipDataSO chipData)
            {
                Skill.SkillData.CoolTime *= chipData.ReduceCooldown;
                Skill.SkillData.SkillRange += chipData.SkillRange;
            }
        }
    }

    public override void SetSkill(Skill skillToEquip, bool isActive)
    {
        base.SetSkill(skillToEquip, isActive);
        RefreshChipEffects();
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
