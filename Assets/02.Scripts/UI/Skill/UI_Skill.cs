using UnityEngine;
using UnityEngine.UI;
public class UI_Skill : MonoBehaviour
{

    public Button Button;
    private Image _icon;
    public Skill Skill { get; private set; }

    private void Awake()
    {
        _icon = GetComponent<Image>();
    }

    public void SetSkill(Skill skillToEquip, bool isActive)
    {
        if (skillToEquip == null) return;
        Button.interactable = isActive;
        Skill = skillToEquip;
        _icon.sprite = Skill.SkillData.Icon;
    }

    public void RemoveSkill()
    {
        Skill = null;
        _icon.sprite = null;
        Button.interactable = false;
    }
}
