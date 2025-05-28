using System;
using System.Collections.Generic;
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

public interface IStatsProvider
{
    float GetStat(StatType statType);
    float CalculateDamage(float baseDamage);
}

public class Stat : IStatsProvider
{
    private readonly Random _rng = new Random();
    public Dictionary<StatType, float> Stats;
    public Stat(Dictionary<StatType, float> statDict)
    {
        Stats = statDict;
    }

    public float GetStat(StatType type)
    {
        return Stats.TryGetValue(type, out float value) ? value : 0f;
    }
    public float CalculateDamage(float baseDamage)
    {
        float attackPower = GetStat(StatType.AttackPower);
        float critChance = GetStat(StatType.CritChance);
        float critDamage = GetStat(StatType.CritDamage);

        bool isCritical = _rng.NextDouble() < critChance;
        float damage = baseDamage + attackPower;

        if (isCritical)
        {
            damage *= critDamage;
        }

        return (float)Math.Round(damage);
    }
}
