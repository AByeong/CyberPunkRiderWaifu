using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkillUIManager : Popup
{
    public List<UI_Skill> AvailableSkills;
    public List<UI_Skill> EquippedSkills;

    private void Start()
    {
       
    }

    private void Update()
    {
        UpdateCooldowns(Time.deltaTime);
    }

    public override void OpenPopup()
    {
        Init();
        Show();
        base.OpenPopup();
    }

    private void Init()
    {
        // EquippedSkills 버튼은 항상 비활성화 (처음 한 번만 설정)
        foreach(UI_Skill equippedSkill in EquippedSkills)
        {
            equippedSkill.Button.interactable = false;
        }

        // AvailableSkills에 SkillManager의 전체 스킬 연결 (처음 한 번만)
        for (int i = 0; i < SkillManager.Instance.Skills.Count; i++)
        {
            AvailableSkills[i].SetSkill(SkillManager.Instance.Skills[i], true);
        }
    }

    public void Show()
    {
        // 모든 Available 버튼 다시 활성화
        foreach(UI_Skill uiSkill in AvailableSkills)
        {
            uiSkill.Button.interactable = true;
        }

        // 장착된 스킬 인덱스를 기준으로 Available/Equipped UI 상태 갱신
        for (int i = 0; i < SkillManager.Instance.EquippedSkills.Count; i++)
        {
            // Skill equipped = SkillManager.Instance.EquippedSkills[i];

            Skill equipped = new Skill();
            equipped.Index = SkillManager.Instance.EquippedSkills[i] != null ? SkillManager.Instance.EquippedSkills[i].Index : -1;
            equipped.SkillData = SkillManager.Instance.EquippedSkills[i]?.SkillData.Clone();
            
            if (equipped.Index != -1)
            {
                // AvailableSkills에서 동일한 스킬은 비활성화
                if (equipped.Index >= 0 && equipped.Index < AvailableSkills.Count)
                {
                    AvailableSkills[equipped.Index].Button.interactable = false;
                }

                // EquippedSkills UI에 표시
                if (i < EquippedSkills.Count)
                {
                    EquippedSkills[i].SetSkill(equipped, true);
                }
            }
            else
            {
                // 스킬이 없으면 UI 초기화
                if (i < EquippedSkills.Count)
                {
                    EquippedSkills[i].RemoveSkill();
                }
            }
        }

    }

    public void EquipSkill(int skillIndex)
    {
        // Skill skillToEquip = AvailableSkills[skillIndex].Skill;
        Skill skillToEquip = new Skill();
        skillToEquip.Index = AvailableSkills[skillIndex].Skill.Index;
        skillToEquip.SkillData = AvailableSkills[skillIndex].Skill.SkillData.Clone();
        
        for (int i = 0; i < SkillManager.Instance.EquippedSkills.Count; i++)
        {
            if (!SkillManager.Instance.EquippedSkillsBool[i]) // 빈 슬롯
            {
                SkillManager.Instance.EquippedSkills[i] = skillToEquip;
                SkillManager.Instance.EquippedSkillsBool[i] = true;

                EquippedSkills[i].SetSkill(skillToEquip, true);
                AvailableSkills[skillIndex].Button.interactable = false;
                SkillManager.Instance.OnSkillChange?.Invoke();

                return;
            }
        }

        Debug.Log("No empty slot found.");
    }

    public void RemoveSkill(int equipIndex)
    {
        Skill skillToRemove = SkillManager.Instance.EquippedSkills[equipIndex];

        if (skillToRemove != null)
        {
            AvailableSkills[skillToRemove.Index].Button.interactable = true;
            EquippedSkills[equipIndex].RemoveSkill();

            SkillManager.Instance.EquippedSkills[equipIndex] = null;
            SkillManager.Instance.EquippedSkillsBool[equipIndex] = false;
        }

        SkillManager.Instance.OnSkillChange?.Invoke();
    }

    private void UpdateCooldowns(float deltaTime)
    {
        foreach(Skill skill in SkillManager.Instance.EquippedSkills)
        {
            if (skill != null && SkillManager.Instance._skillCurrentCooldowns.ContainsKey(skill))
            {
                SkillManager.Instance._skillCurrentCooldowns[skill] += deltaTime;
            }
        }
    }
}
