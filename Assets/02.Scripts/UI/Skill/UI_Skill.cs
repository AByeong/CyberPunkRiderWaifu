using UnityEngine;
using UnityEngine.UI;
public class UI_Skill : MonoBehaviour
{
    public float _skillCooldown;
    public float _skillRange;

    private Image _icon;
    public Skill Skill { get; private set; }

    private void Awake()
    {
        _icon = GetComponent<Image>();
    }

    public void SetSkill(Skill skillToEquip)
    {
        Skill = skillToEquip;

        _icon.sprite = Skill.SkillData.Icon;
        _skillRange = Skill.SkillData.SkillRange;
        _skillCooldown = Skill.SkillData.CoolTime;
    }

    public void RemoveSkill()
    {
        Skill = null;
        _icon.sprite = null;
    }
}
