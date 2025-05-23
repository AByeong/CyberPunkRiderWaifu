using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
public class SkillManager : Singleton<SkillManager>
{
    public SkillDataList DataList;

    [SerializeField]
    [ItemCanBeNull]
    private List<Skill> equippedSkills;
    public Dictionary<Skill, float> _skillCurrentCooldowns = new Dictionary<Skill, float>();
    public List<Skill> EquippedSkills => equippedSkills;

    private void Awake()
    {
        // 완전히 초기화
        equippedSkills = new List<Skill>();

        // 4개의 null 슬롯 추가
        for (int i = 0; i < 4; i++)
        {
            equippedSkills.Add(null);
        }

        Debug.Log($"SkillManager Awake: Initialized with {equippedSkills.Count} empty slots");
    }
    // private void Start()
    // {
    //     Debug.Log("=== SkillManager Start Debug ===");
    //     Debug.Log($"EquippedSkills count: {equippedSkills.Count}");
    //
    //     for (int i = 0; i < equippedSkills.Count; i++)
    //     {
    //         if (equippedSkills[i] == null)
    //         {
    //             Debug.Log($"Start - Slot {i}: NULL");
    //         }
    //         else
    //         {
    //             Debug.Log($"Start - Slot {i}: {equippedSkills[i].SkillData?.SkillName} (Index: {equippedSkills[i].Index}) - Hash: {equippedSkills[i].GetHashCode()}");
    //         }
    //     }
    // }
    private void Update()
    {
        UpdateSkillCooldowns(Time.deltaTime);
    }

    private void UpdateSkillCooldowns(float deltaTime)
    {
        foreach(Skill skill in equippedSkills)
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

        if (keyNumber >= 0 && keyNumber < equippedSkills.Count && equippedSkills[keyNumber] != null)
        {
            Skill skillToUse = equippedSkills[keyNumber];
            if (skillToUse.CanUse())
            {
                skillToUse.Use();
                return true;
            }
        }

        return false;
    }
}
