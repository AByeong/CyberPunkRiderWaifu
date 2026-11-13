using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UI_Skill : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button Button;
    public Image Icon;
    public Skill Skill { get; private set; }
    public Skill SkillBase;
    public virtual void SetSkill(Skill skillToEquip, bool isActive)
    {
        if (skillToEquip == null) return;
        Button.interactable = isActive;
        SkillBase = skillToEquip;
        ResetBaseSkill(skillToEquip);
        Icon.sprite = Skill.SkillData.Icon;
    }
    protected void ResetBaseSkill(Skill skillToEquip)
    {
        Skill = new Skill();
        Skill.Index = skillToEquip.Index;

        // 칩 효과가 이미 반영된 데이터를 기준으로 삼으면 열 때마다 효과가 중첩되므로,
        // 가능하면 SkillManager가 가진 원본(칩 미적용) 데이터에서 복제한다
        SkillData source = skillToEquip.SkillData;
        if (SkillManager.Instance != null && skillToEquip.Index >= 0 && skillToEquip.Index < SkillManager.Instance.Skills.Count)
        {
            source = SkillManager.Instance.Skills[skillToEquip.Index].SkillData;
        }
        Skill.SkillData = source.Clone();
    }

    public virtual void RemoveSkill()
    {
        Skill = null;
        Icon.sprite = null;
        Button.interactable = false;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        UI_SkillInspector.Instance.Hovered(Skill);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        UI_SkillInspector.Instance.Exit();
    }
}
