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
        Skill.SkillData = skillToEquip.SkillData.Clone();
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
