using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{
    private List<Skill> _availableSkills;
    private List<Skill> _equippedSkills;
    private Dictionary<Skill, float> _skillCooldowns;

    public void EquipSkill(string skillId, int slotIndex)
    {

    }

    public void UnequipSkill(int slotIndex)
    {

    }

    public bool UseSkill(int slotIndex)
    {
        return _equippedSkills[slotIndex].TryUseSkill();
    }

    public void UpdateCooldowns(float deltaTime)
    {

    }
}
