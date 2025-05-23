using UnityEngine;
using UnityEngine.UI;
public class UI_Skill : MonoBehaviour
{
    public float _skillCooldown;
    public float _skillRange;
    private Image _icon;
    private Skill _skill;

    private void Awake()
    {
        _icon = GetComponent<Image>();
    }

    public void SetSkill(Skill skillToEquip)
    {
        _skill = skillToEquip;
        Debug.Log(_icon);
        _icon.sprite = _skill.SkillData.Icon;
        _skillRange = _skill.SkillData.SkillRange;
        _skillCooldown = _skill.SkillData.CoolTime;
    }

    public void RemoveSkill()
    {
        _skill = null;
        _icon.sprite = null;
    }
}
