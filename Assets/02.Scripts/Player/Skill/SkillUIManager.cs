// SkillUIManager.cs

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
        for (int i = 0; i < SkillManager.Instance.Skills.Count; i++)
        {
            AvailableSkills[i].SetSkill(SkillManager.Instance.Skills[i], true);
        }
        // 2단계: EquippedSkills는 전부 비활성화
        foreach(UI_Skill equippedSkill in EquippedSkills)
        {
            equippedSkill.Button.interactable = false;
        }
        // 3단계: 장착된 스킬만 버튼 비활성화
        foreach(Skill equipped in SkillManager.Instance.EquippedSkills)
        {
            if (equipped != null && equipped.Index > 0)
            {
                AvailableSkills[equipped.Index].Button.interactable = false;
            }
        }


    }
    private void Update()
    {
        UpdateCooldowns(Time.deltaTime);
    }


    public void Show()
    { // 1단계: 모두 초기화 및 활성화
        for (int i = 0; i < SkillManager.Instance.Skills.Count; i++)
        {
            AvailableSkills[i].SetSkill(SkillManager.Instance.Skills[i], true);
        }

        // 2단계: 장착된 스킬만 버튼 비활성화
        foreach(Skill equipped in SkillManager.Instance.EquippedSkills)
        {
            if (equipped != null)
            {
                AvailableSkills[equipped.Index].Button.interactable = false;
            }
        }
        gameObject.SetActive(true);
    }
    public void EquipSkill(int skillIndex)
    {

        Skill skillToEquip = AvailableSkills[skillIndex].Skill;

        for (int i = 0; i < SkillManager.Instance.EquippedSkills.Count; i++)
        {
            Skill slot = SkillManager.Instance.EquippedSkills[i];

            if (!SkillManager.Instance.EquippedSkillsBool[i]) // 빈 슬롯을 찾았을 때
            {
                Debug.Log($"Found empty slot at index {i}!");

                // 여기서 실제 장착 로직
                SkillManager.Instance.EquippedSkills[i] = skillToEquip;
                EquippedSkills[i].SetSkill(skillToEquip, true);

                AvailableSkills[skillIndex].Button.interactable = false;
                SkillManager.Instance.EquippedSkillsBool[i] = true;

                return;
            }
        }

        Debug.Log("No empty slot found.");
    }

    public void RemoveSkill(int equipIndex)
    {
        Skill skillToRemove = SkillManager.Instance.EquippedSkills[equipIndex];

        AvailableSkills[skillToRemove.Index].Button.interactable = true;
        EquippedSkills[equipIndex].RemoveSkill();
        // EquippedSkills[equipIndex].GetComponent<Button>().interactable = false;
        SkillManager.Instance.EquippedSkills[equipIndex] = null;
        SkillManager.Instance.EquippedSkillsBool[equipIndex] = false;
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
