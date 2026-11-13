using UnityEngine.EventSystems;

public class UI_EquipSkill : UI_Skill
{
    public UI_ChipSlot[] ChipSlots;
    public int Index = -1;


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

        // 3. 계산 결과를 실제로 게임에서 쓰는 장착 스킬(SkillManager.EquippedSkills)에 반영
        //    (기존에는 UI 표시용 복제본에만 적용되어 실제 쿨타임/범위는 변하지 않았음)
        ApplyToEquippedSkill();
    }

    private void ApplyToEquippedSkill()
    {
        if (SkillManager.Instance == null || Skill == null) return;

        var equipped = SkillManager.Instance.EquippedSkills;
        if (Index < 0 || Index >= equipped.Count) return;

        Skill realSkill = equipped[Index];
        if (realSkill == null || realSkill.Index != Skill.Index) return;

        // Skill 객체는 그대로 두고 데이터만 교체해 진행 중인 쿨타임은 유지한다
        realSkill.SkillData = Skill.SkillData.Clone();
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
