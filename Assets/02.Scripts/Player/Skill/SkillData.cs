using System;
using UnityEngine;

public enum SkillRange
{
    Short,
    Medium,
    Long,
    Global
}

[Serializable]
public class SkillData
{
    public string SkillName;
    public string TriggerName;
    public EPlayerState PlayerState;
    public float SkillSpeed;
    public float CoolTime;
    public bool IsUpgraded;
    public bool IsGroundSkill;
    public EDamageType DamageType;
    public float SkillRange;
    public float SkillDamage;
    public Sprite Icon;
    public string SkillDescription;
    public SkillData Clone()
    {
        return new SkillData()
        {
            SkillName = this.SkillName,
            TriggerName = this.TriggerName,
            PlayerState = this.PlayerState,
            SkillSpeed = this.SkillSpeed,
            CoolTime = this.CoolTime,
            IsUpgraded = this.IsUpgraded,
            IsGroundSkill = this.IsGroundSkill,
            DamageType = this.DamageType,
            SkillRange = this.SkillRange,
            SkillDamage = this.SkillDamage,
            Icon = this.Icon,
            SkillDescription = this.SkillDescription
        };
    }
}