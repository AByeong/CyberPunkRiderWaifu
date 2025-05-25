// SkillUIManager.cs

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public class SkillUIManager : MonoBehaviour
{
    public List<UI_Skill> AvailableSkills;
    public List<UI_Skill> EquippedSkills;

    private void Start()
    {
        Debug.Log(SkillManager.Instance.DataList.SkillData);

        // 1단계: 모두 초기화 및 활성화
        for (int i = 0; i < SkillManager.Instance.DataList.SkillData.Count; i++)
        {
            Skill skill = new Skill {SkillData = SkillManager.Instance.DataList.SkillData[i], Index = i};
            if (SkillManager.Instance.EquippedSkills.Contains(skill))
            {
                AvailableSkills[i].SetSkill(skill, true);
            }
            AvailableSkills[i].SetSkill(skill, true);
        }

        // 2단계: 장착된 스킬만 버튼 비활성화
        foreach(Skill equipped in SkillManager.Instance.EquippedSkills)
        {
            if (equipped != null)
            {
                AvailableSkills[equipped.Index].Button.interactable = false;
            }
        }
    }
    private void Update()
    {
        UpdateCooldowns(Time.deltaTime);
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

                AvailableSkills[skillIndex].GetComponent<Button>().interactable = false;
                SkillManager.Instance.EquippedSkillsBool[i] = true;

                return;
            }
        }

        Debug.Log("No empty slot found.");
    }

    public void UnequipSkill(int equipIndex)
    {
        Skill skillToUnequip = SkillManager.Instance.EquippedSkills[equipIndex];
        AvailableSkills[skillToUnequip.Index].GetComponent<Button>().interactable = true;
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
