using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UI_Skill : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button Button;
    public Image Icon;
    public Skill Skill { get; private set; }

    public virtual void SetSkill(Skill skillToEquip, bool isActive)
    {
        if (skillToEquip == null) return;
        Button.interactable = isActive;
        Skill = skillToEquip;
        Icon.sprite = Skill.SkillData.Icon;
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
