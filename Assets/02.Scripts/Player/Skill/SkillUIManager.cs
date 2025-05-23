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
        Skill skillToEquip = AvailableSkills[skillIndex].Skill;
        for (int i = 0; i < SkillManager.Instance.EquippedSkills.Count; i++)
        {
            if (SkillManager.Instance.EquippedSkills[i] == null)
            {
                SkillManager.Instance.EquippedSkills[i] = skillToEquip;
                EquippedSkills[i].SetSkill(skillToEquip);
                EquippedSkills[i].GetComponent<Button>().interactable = true;
                AvailableSkills[skillIndex].GetComponent<Button>().interactable = false;

                if (!SkillManager.Instance._skillCurrentCooldowns.ContainsKey(skillToEquip))
                    SkillManager.Instance._skillCurrentCooldowns.Add(skillToEquip, skillToEquip.SkillData.CoolTime);
                else
                    SkillManager.Instance._skillCurrentCooldowns[skillToEquip] = skillToEquip.SkillData.CoolTime;

                Debug.Log($"Added 1 to slot {i}");
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
