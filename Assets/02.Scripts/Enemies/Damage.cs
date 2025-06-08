using UnityEngine;

public enum EDamageType
{
    Normal,
    Airborne,
    Down
}
public enum EDamageCriType
{
    Normal,
    Critical,
}
public struct Damage
{
    public GameObject From;
    public EDamageType DamageType;
    public EDamageCriType DamageCriType;
    public float DamageForce;
    public int DamageValue;
    public float AirRiseAmount;

    public void InitAirRiseAmount()
    {
        AirRiseAmount = 2f;
    }
}
