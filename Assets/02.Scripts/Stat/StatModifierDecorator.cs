/// <summary>
///     스탯 수치 증가 데커레이터 (예: 무기, 방어구, 버프)
/// </summary>
public class StatModifierDecorator : StatsDecorator
{
    private readonly StatType _type;
    private readonly float _value;

    public StatModifierDecorator(IStatsProvider wrapped, StatType type, float value) : base(wrapped)
    {
        _type = type;
        _value = value;
    }

    public override float GetStat(StatType statType)
    {
        if (statType == _type)
            return base.GetStat(statType) + _value;
        return base.GetStat(statType);
    }
}
