using MoreMountains.Tools;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{
    public SkillDataSO dataSO;
    private List<Skill> _availableSkills;
    private List<Skill> _equippedSkills;
    private Dictionary<Skill, float> _skillCurrentCooldowns;

    private void Awake()
    {
        

    }
    public void EquipSkill(int skillIndex, int equipIndex)
    {
        _equippedSkills[equipIndex - 1] = _availableSkills[skillIndex - 1];
    }

    public void UnequipSkill(int equipIndex)
    {
        _equippedSkills.RemoveAt(equipIndex - 1);
    }

    public void UseSkill(KeyCode key)
    {
        switch(key)
        {
            case KeyCode.Alpha1:
                if(_skillCurrentCooldowns[_equippedSkills[0]] >= _equippedSkills[0].SkillData.CoolTime )
                {
                    _skillCurrentCooldowns[_equippedSkills[0]] = 0.0f;
                }
                break;
            case KeyCode.Alpha2:
                if (_skillCurrentCooldowns[_equippedSkills[1]] >= _equippedSkills[1].SkillData.CoolTime)
                {
                    _skillCurrentCooldowns[_equippedSkills[1]] = 0.0f;
                }
                break;
            case KeyCode.Alpha3:
                if (_skillCurrentCooldowns[_equippedSkills[2]] >= _equippedSkills[2].SkillData.CoolTime)
                {
                    _skillCurrentCooldowns[_equippedSkills[2]] = 0.0f;
                }
                break;
            case KeyCode.Alpha4:
                if (_skillCurrentCooldowns[_equippedSkills[3]] >= _equippedSkills[3].SkillData.CoolTime)
                {
                    _skillCurrentCooldowns[_equippedSkills[3]] = 0.0f;
                }
                break;
        }
    }

    public void UpdateCooldowns(float deltaTime)
    {
        foreach(Skill skill in _equippedSkills)
        {
            _skillCurrentCooldowns[skill] += deltaTime;
        }
    }
}
