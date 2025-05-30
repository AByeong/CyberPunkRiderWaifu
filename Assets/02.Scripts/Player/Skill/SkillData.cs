using System;
using UnityEngine;
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
    public float SkillRange;
    public float SkillDamage;
    public Sprite Icon;
}
