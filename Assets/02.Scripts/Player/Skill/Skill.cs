using System;
using UnityEngine;
[Serializable]
public class Skill
{
    public int Index;
    public SkillData SkillData;
    private float _cooldownTimer;
    
    private bool _isActive = false;

    public Skill()
    {
        Index = -1;
        _cooldownTimer = 100;
    }
    public void UpdateCooldown(float deltaTime)
    {
        _cooldownTimer += deltaTime;
    }

    public bool CanUse()
    {
        return _cooldownTimer >= SkillData.CoolTime;
    }

    public void ClearCooltime()
    {
        _cooldownTimer = 1000;
    }

    public void Use()
    {
        _cooldownTimer = 0f;
        Debug.Log(SkillData.CoolTime);
    }
}
