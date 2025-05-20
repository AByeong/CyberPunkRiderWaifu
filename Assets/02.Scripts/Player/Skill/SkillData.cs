using System;
using UnityEngine;

[Serializable]
public class SkillData
{
    public string SkillName;
    public KeyCode KeyButton;
    public string TriggerName;
    public float SkillSpeed;
    public float CoolTime;
    public bool IsUpgraded;
    public bool IsGroundSkill;
}
