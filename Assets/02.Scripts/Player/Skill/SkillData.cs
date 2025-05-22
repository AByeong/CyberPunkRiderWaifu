using System;
[Serializable]
public class SkillData
{
    public string SkillName;
    public string TriggerName;
    public float SkillSpeed;
    public float CoolTime;
    public bool IsUpgraded;
    public bool IsGroundSkill;
    public float SkillRange = 1f;
}
