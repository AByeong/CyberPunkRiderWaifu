using UnityEngine;
using UnityEngine.UI;
public class UI_Skill : MonoBehaviour
{

    public float SkillRange;
    public float SKillCoolDownTime;
    private Image _icon;
    public Skill Skill { get; private set; }

    private void Awake()
    {
        _icon = GetComponent<Image>();
    }

    public void SetSkill(Skill skillToEquip)
    {
        Skill = skillToEquip;
        SkillRange = skillToEquip.SkillData.SkillRange;
        SKillCoolDownTime = skillToEquip.SkillData.CoolTime;
        _icon.sprite = Skill.SkillData.Icon;
    }

    public void RemoveSkill()
    {
        Skill = null;
        _icon.sprite = null;
    }
}
