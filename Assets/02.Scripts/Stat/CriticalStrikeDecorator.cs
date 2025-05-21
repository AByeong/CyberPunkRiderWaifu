using System;
/// <summary>
///     크리티컬 데미지 커스터마이징 (선택사항)
/// </summary>
public class CriticalStrikeDecorator : StatsDecorator
{
    private readonly Random _rng = new Random();

    public CriticalStrikeDecorator(IStatsProvider wrapped) : base(wrapped) { }

    public override float CalculateDamage(float baseDamage)
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
