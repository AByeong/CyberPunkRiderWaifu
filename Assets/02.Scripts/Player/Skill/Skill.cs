using System;
[Serializable]
public class Skill
{
    public int Index;
    public SkillData SkillData;
    private float _cooldownTimer;

    private bool _isActive = false;
    public void UpdateCooldown(float deltaTime)
    {
        _cooldownTimer += deltaTime;
    }

    public bool CanUse()
    {
        return _cooldownTimer >= SkillData.CoolTime;
    }

    public void Use()
    {
        _cooldownTimer = 0f;
        // 실제 스킬 실행 로직 (이펙트, 애니메이션 등)
    }
}
