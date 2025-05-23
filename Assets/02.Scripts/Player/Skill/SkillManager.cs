using System.Collections.Generic;
using UnityEngine;
public class SkillManager : Singleton<SkillManager>
{
    public SkillDataList DataList;
    public List<Skill> Skills;
    public List<Skill> EquippedSkills;
    public List<bool> EquippedSkillsBool;
    // private List<Skill> equippedSkills;
    public Dictionary<Skill, float> _skillCurrentCooldowns = new Dictionary<Skill, float>();
    private void Awake()
    {
        base.Awake();
        // 완전히 초기화
        EquippedSkills = new List<Skill>();
        EquippedSkillsBool = new List<bool>();
        for (int i = 0; i < 4; i++)
        {
            EquippedSkills.Add(null);
            EquippedSkillsBool.Add(false);
        }
    }
    private void Update()
    {
        UpdateSkillCooldowns(Time.deltaTime);
    }

    private void UpdateSkillCooldowns(float deltaTime)
    {
        foreach(Skill skill in EquippedSkills)
        {
            skill?.UpdateCooldown(deltaTime);
        }
    }

    public bool UseSkill(KeyCode key, out int keyNumber)
    {
        keyNumber = key switch
        {
            KeyCode.Alpha1 => 0,
            KeyCode.Alpha2 => 1,
            KeyCode.Alpha3 => 2,
            KeyCode.Alpha4 => 3,
            _ => -1
        };

        if (keyNumber >= 0 && keyNumber < EquippedSkills.Count && EquippedSkills[keyNumber] != null)
        {
            Skill skillToUse = EquippedSkills[keyNumber];
            if (skillToUse.CanUse())
            {
                skillToUse.Use();
                return true;
            }
        }

        return false;
    }
}
