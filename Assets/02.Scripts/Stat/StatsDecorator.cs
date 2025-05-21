/// <summary>
///     데커레이터 추상 클래스
/// </summary>
public abstract class StatsDecorator : IStatsProvider
{
    protected IStatsProvider _wrapped;

    protected StatsDecorator(IStatsProvider wrapped)
    {
        _wrapped = wrapped;
    }

    public virtual float GetStat(StatType statType)
    {
        return _wrapped.GetStat(statType);
    }
    public virtual float CalculateDamage(float baseDamage)
    {
        return _wrapped.CalculateDamage(baseDamage);
    }
}
