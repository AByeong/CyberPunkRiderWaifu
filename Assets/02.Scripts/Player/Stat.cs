using System;

public enum StatType
{
    Health,
    MaxHealth,
    AttackPower,
    Defense,
    Speed,
    AttackSpeed,
    CritChance,
    CritDamage
}

public class PlayerStats
{
    private float _health;
    private float _maxHealth;

    private float _attackPower;
    private float _defense;
    private float _speed;
    private float _attackSpeed;
    private float _critChance;   // 0.0 ~ 1.0 (예: 0.25 = 25%)
    private float _critDamage;   // 배율 (예: 1.5 = 150%)

    private Random _rng = new Random();

    public PlayerStats(PlayerSO PlayerData)
    {
        // _maxHealth = maxHealth;
        // _health = maxHealth;
        //
        // _attackPower = attackPower;
        // _defense = defense;
        // _speed = speed;
        // _attackSpeed = 
        //
        // _critChance = critChance;
        // _critDamage = critDamage;
    }

    public float GetStatValue(StatType statType)
    {
        return statType switch
        {
            StatType.Health => _health,
            StatType.MaxHealth => _maxHealth,
            StatType.AttackPower => _attackPower,
            StatType.Defense => _defense,
            StatType.Speed => _speed,
            StatType.CritChance => _critChance,
            StatType.CritDamage => _critDamage,
            _ => throw new ArgumentOutOfRangeException(nameof(statType), $"Invalid stat type: {statType}")
        };
    }

    public void ModifyStat(StatType statType, float value)
    {
        switch (statType)
        {
            case StatType.Health:
                _health = Math.Clamp(_health + value, 0, _maxHealth);
                break;
            case StatType.MaxHealth:
                _maxHealth = Math.Max(1, (_maxHealth + value));
                _health = Math.Min(_health, _maxHealth);
                break;  
            case StatType.AttackPower:
                _attackPower += value;
                break;
            case StatType.Defense:
                _defense += value;
                break;
            case StatType.Speed:
                _speed += value;
                break;
            case StatType.AttackSpeed:
                _attackSpeed += value;
                break;
            case StatType.CritChance:
                _critChance = Math.Clamp(_critChance + value, 0f, 1f);
                break;
            case StatType.CritDamage:
                _critDamage += value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(statType), $"Invalid stat type: {statType}");
        }
    }

    public float CalculateDamage(float baseDamage)
    {
        bool isCritical = _rng.NextDouble() < _critChance;
        float damage = baseDamage + _attackPower;
        if (isCritical)
        {
            damage *= _critDamage;
        }

        return (float)Math.Round(damage);
    }
}
