using System;
using System.Collections.Generic;
using Gamekit3D;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public class SkillManager : Singleton<SkillManager>
{
    public SkillDataList DataList;

    public List<UI_Skill> AvailableSkills;
    public List<UI_Skill> EquippedSkills;
    private List<Skill> _equippedSkills = new List<Skill>();
    private PlayerController _playerController;
    private Dictionary<Skill, float> _skillCurrentCooldowns = new Dictionary<Skill, float>();

    public List<Skill> EquippedSkill { get; } = new List<Skill>();
    // public Skill EquipSkill1 => _equippedSkills[0];
    // public Skill EquipSkill2 => _equippedSkills[1];
    // public Skill EquipSkill3 => _equippedSkills[2];
    // public Skill EquipSkill4 => _equippedSkills[3];


    private void Start()
    {
        // 빈 슬롯으로 _equippedSkills 초기화
        for (int i = 0; i < 4; i++)
        {
            _equippedSkills.Add(null);
            EquippedSkills[i].GetComponent<Button>().interactable = false;
        }

        // 사용 가능한 스킬 초기화
        int skillIndex = 0;
        foreach(SkillData data in DataList.SkillData)
        {
            Skill skill = new Skill();
            skill.SkillData = data;
            skill.Index = skillIndex;
            AvailableSkills[skillIndex].SetSkill(skill);
            skillIndex++;
            EquippedSkill.Add(skill);
        }

        // 기본 스킬 장착
        // EquipSkill(1, 1);
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        _playerController = player.GetComponent<PlayerController>();
    }

    private void Update()
    {
        UpdateCooldowns(Time.deltaTime);
    }

    public void EquipSkill(int skillIndex)
    {
        if (skillIndex <= 0 || skillIndex > EquippedSkill.Count)
        {
            Debug.LogError("잘못된 스킬 인덱스입니다.");
            return;
        }

        Skill skillToEquip = EquippedSkill[skillIndex - 1];
        // 비어있는 슬롯 찾기
        for (int i = 0; i < _equippedSkills.Count; i++)
        {
            if (_equippedSkills[i] == null)
            {
                _equippedSkills[i] = skillToEquip;
                EquippedSkills[i].SetSkill(skillToEquip);
                EquippedSkills[i].GetComponent<Button>().interactable = true;

                AvailableSkills[skillToEquip.Index].GetComponent<Button>().interactable = false;

                // 쿨다운 초기화
                if (!_skillCurrentCooldowns.ContainsKey(skillToEquip))
                {
                    _skillCurrentCooldowns.Add(skillToEquip, skillToEquip.SkillData.CoolTime);
                }
                else
                {
                    _skillCurrentCooldowns[skillToEquip] = skillToEquip.SkillData.CoolTime;
                }

                return; // 장착했으므로 함수 종료
            }
        }

        // 비어있는 슬롯이 없음
        Debug.Log("장착 가능한 빈 슬롯이 없습니다.");
    }


    public void UnequipSkill(int equipIndex)
    {
        if (equipIndex <= 0 || equipIndex > 4)
        {
            Debug.LogError("잘못된 장착 인덱스입니다.");
            return;
        }
        Skill skillToUnequip = _equippedSkills[equipIndex - 1];
        AvailableSkills[skillToUnequip.Index].GetComponent<Button>().interactable = true;
        EquippedSkills[equipIndex - 1].RemoveSkill();
        EquippedSkills[equipIndex - 1].GetComponent<Button>().interactable = false;
        _equippedSkills[equipIndex - 1] = null;
    }

    public bool UseSkill(KeyCode key, out int keyNumber)
    {
        keyNumber = -1;

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
                return false; // 잘못된 키
        }

        // 슬롯 범위 확인 및 스킬 존재 여부 확인
        if (keyNumber >= 0 && keyNumber < _equippedSkills.Count && _equippedSkills[keyNumber] != null)
        {
            Skill skillToUse = _equippedSkills[keyNumber];

            // 쿨타임 체크
            if (_skillCurrentCooldowns[skillToUse] >= skillToUse.SkillData.CoolTime)
            {
                Debug.Log($"{keyNumber + 1}번 스킬 발동");
                _skillCurrentCooldowns[skillToUse] = 0.0f;
                Debug.Log($"Using {skillToUse.SkillData.TriggerName}");
                _playerController._animator.SetTrigger(skillToUse.SkillData.TriggerName);
                return true;
            }
            Debug.Log($"{keyNumber + 1}번 스킬 쿨다운 중: {_skillCurrentCooldowns[skillToUse]}/{skillToUse.SkillData.CoolTime}");
        }

        return false; // 스킬 없음 또는 쿨다운 중
    }


    private void UpdateCooldowns(float deltaTime)
    {
        foreach(Skill skill in _equippedSkills)
        {
            if (skill != null && _skillCurrentCooldowns.ContainsKey(skill))
            {
                _skillCurrentCooldowns[skill] += deltaTime;
            }
        }
    }
}
