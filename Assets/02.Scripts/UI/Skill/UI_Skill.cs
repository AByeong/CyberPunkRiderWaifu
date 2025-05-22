using UnityEngine;
using UnityEngine.UI;
public class UI_Skill : MonoBehaviour
{
    private Image _icon;
    private Skill _skill;
    private void Start()
    {
        _icon = GetComponent<Image>();
    }

    public void SetSkill(Skill skillToEquip)
    {
        _skill = skillToEquip;
    }
}
