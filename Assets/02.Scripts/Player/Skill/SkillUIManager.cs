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
        for (int i = 0; i < SkillManager.Instance.DataList.SkillData.Count; i++)
        {
            Skill skill = new Skill {SkillData = SkillManager.Instance.DataList.SkillData[i], Index = i};
            AvailableSkills[i].SetSkill(skill);
        }

        for (int i = 0; i < 4; i++)
        {
            EquippedSkills[i].GetComponent<Button>().interactable = false;
        }
    }

    private void Update()
    {
        UpdateCooldowns(Time.deltaTime);
    }

    public void EquipSkill(int skillIndex)
    {
        Debug.Log("=== EquipSkill Debug Start ===");
        Debug.Log($"skillIndex parameter: {skillIndex}");

        Skill skillToEquip = AvailableSkills[skillIndex].Skill;
        Debug.Log($"Skill to equip: {skillToEquip?.SkillData?.SkillName ?? "NULL"}, Index: {skillToEquip?.Index ?? -1}");

        for (int i = 0; i < SkillManager.Instance.EquippedSkills.Count; i++)
        {
            Skill slot = SkillManager.Instance.EquippedSkills[i];
            Debug.Log($"Slot {i}: {(slot == null ? "Empty (null)" : $"Occupied - {slot.SkillData?.SkillName}")}");

            if (slot == null) // 빈 슬롯을 찾았을 때
            {
                Debug.Log($"Found empty slot at index {i}!");

                // 여기서 실제 장착 로직
                SkillManager.Instance.EquippedSkills[i] = skillToEquip;
                EquippedSkills[i].SetSkill(skillToEquip);
                EquippedSkills[i].GetComponent<Button>().interactable = true;
                AvailableSkills[skillIndex].GetComponent<Button>().interactable = false;

                Debug.Log($"Successfully equipped skill to slot {i}");
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
        EquippedSkills[equipIndex].GetComponent<Button>().interactable = false;
        SkillManager.Instance.EquippedSkills[equipIndex] = null;
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
