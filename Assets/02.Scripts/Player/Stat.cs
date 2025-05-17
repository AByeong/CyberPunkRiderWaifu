using System;

public enum StatType
{
    Health,
    MaxHealth,
    Stamina,
    MaxStamina,
    AttackPower,
    Defense,
    Speed,
    CritChance,
    CritDamage
}

public class PlayerStats
{
    private int _health;
    private int _maxHealth;

    private int _stamina;
    private int _maxStamina;

    private int _attackPower;
    private int _defense;
    private int _speed;

    private float _critChance;   // 0.0 ~ 1.0 (예: 0.25 = 25%)
    private float _critDamage;   // 배율 (예: 1.5 = 150%)

    private Random _rng = new Random();

    public PlayerStats(int maxHealth, int maxStamina, int attackPower, int defense, int speed, float critChance, float critDamage)
    {
        _maxHealth = maxHealth;
        _health = maxHealth;

        _maxStamina = maxStamina;
        _stamina = maxStamina;

        _attackPower = attackPower;
        _defense = defense;
        _speed = speed;

        _critChance = critChance;
        _critDamage = critDamage;
    }

    public float GetStatValue(StatType statType)
    {
        return statType switch
        {
            StatType.Health => _health,
            StatType.MaxHealth => _maxHealth,
            StatType.Stamina => _stamina,
            StatType.MaxStamina => _maxStamina,
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
                _health = (int)Math.Clamp(_health + value, 0, _maxHealth);
                break;
            case StatType.MaxHealth:
                _maxHealth = Math.Max(1, (int)(_maxHealth + value));
                _health = Math.Min(_health, _maxHealth);
                break;
            case StatType.Stamina:
                _stamina = (int)Math.Clamp(_stamina + value, 0, _maxStamina);
                break;
            case StatType.MaxStamina:
                _maxStamina = Math.Max(1, (int)(_maxStamina + value));
                _stamina = Math.Min(_stamina, _maxStamina);
                break;
            case StatType.AttackPower:
                _attackPower += (int)value;
                break;
            case StatType.Defense:
                _defense += (int)value;
                break;
            case StatType.Speed:
                _speed += (int)value;
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

    public int CalculateDamage(int baseDamage)
    {
        bool isCritical = _rng.NextDouble() < _critChance;
        float damage = baseDamage + _attackPower;
        if (isCritical)
        {
            damage *= _critDamage;
        }

        return (int)Math.Round(damage);
    }
}
