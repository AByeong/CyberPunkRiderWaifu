using System;
using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;
using Gamekit3D;

[Serializable]
public class SkillManager : Singleton<SkillManager>
{
    public SkillDataSO DataSO;
    private List<Skill> _availableSkills = new List<Skill>();
    private List<Skill> _equippedSkills = new List<Skill>();
    private Dictionary<Skill, float> _skillCurrentCooldowns = new Dictionary<Skill, float>();
    private Gamekit3D.PlayerController _playerController;

    private void Start()
    {
        // 빈 슬롯으로 _equippedSkills 초기화
        for (int i = 0; i < 4; i++)
        {
            _equippedSkills.Add(null);
        }

        // 사용 가능한 스킬 초기화
        int skillIndex = 0;
        foreach (SkillData data in DataSO.SkillData)
        {
            Skill skill = new Skill();
            skill.SkillData = data;
            skill.Index = skillIndex;
            skillIndex++;
            _availableSkills.Add(skill);
        }

        // 기본 스킬 장착
        EquipSkill(1, 1);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _playerController = player.GetComponent<Gamekit3D.PlayerController>();
    }

    public void EquipSkill(int skillIndex, int equipIndex)
    {
        if (skillIndex <= 0 || skillIndex > _availableSkills.Count || 
            equipIndex <= 0 || equipIndex > 4)
        {
            Debug.LogError("잘못된 스킬 또는 장착 인덱스입니다.");
            return;
        }

        Skill skillToEquip = _availableSkills[skillIndex - 1];
        _equippedSkills[equipIndex - 1] = skillToEquip;
        
        // 쿨다운 초기화
        if (!_skillCurrentCooldowns.ContainsKey(skillToEquip))
        {
            _skillCurrentCooldowns.Add(skillToEquip, skillToEquip.SkillData.CoolTime);
        }
        else
        {
            _skillCurrentCooldowns[skillToEquip] = skillToEquip.SkillData.CoolTime;
        }
    }

    public void UnequipSkill(int equipIndex)
    {
        if (equipIndex <= 0 || equipIndex > 4)
        {
            Debug.LogError("잘못된 장착 인덱스입니다.");
            return;
        }

        _equippedSkills[equipIndex - 1] = null;
    }

    public void UseSkill(KeyCode key)
    {
        int keyNumber = 0;
        switch (key)
        {
            case KeyCode.Alpha1:
                keyNumber = 0;
                break;
            case KeyCode.Alpha2:
                keyNumber = 1;
                break;
            case KeyCode.Alpha3:
                keyNumber = 2;
                break;
            case KeyCode.Alpha4:
                keyNumber = 3;
                break;
            default:
                return;
        }

        // 해당 슬롯에 스킬이 장착되어 있는지 확인
        if (keyNumber >= 0 && keyNumber < _equippedSkills.Count && _equippedSkills[keyNumber] != null)
        {
            Skill skillToUse = _equippedSkills[keyNumber];
            
            // 쿨다운 확인
            if (_skillCurrentCooldowns[skillToUse] >= skillToUse.SkillData.CoolTime)
            {
                Debug.Log($"{keyNumber + 1}번 스킬 발동");
                _skillCurrentCooldowns[skillToUse] = 0.0f;
                Debug.Log($"Using {_equippedSkills[keyNumber].SkillData.TriggerName}");
                _playerController.m_Animator.SetTrigger($"{_equippedSkills[keyNumber].SkillData.TriggerName}");
            }
            else
            {
                Debug.Log($"{keyNumber + 1}번 스킬 쿨다운 중: {_skillCurrentCooldowns[skillToUse]}/{skillToUse.SkillData.CoolTime}");
            }
        }
    }

    private void Update()
    {
        UpdateCooldowns(Time.deltaTime);

        // 키 입력 처리
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseSkill(KeyCode.Alpha1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseSkill(KeyCode.Alpha2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseSkill(KeyCode.Alpha3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UseSkill(KeyCode.Alpha4);
        }
    }

    private void UpdateCooldowns(float deltaTime)
    {
        foreach (Skill skill in _equippedSkills)
        {
            if (skill != null && _skillCurrentCooldowns.ContainsKey(skill))
            {
                _skillCurrentCooldowns[skill] += deltaTime;
            }
        }
    }
}