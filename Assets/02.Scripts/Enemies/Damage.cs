using UnityEngine;

public enum EDamageType
{
    Normal,
    Airborne,
    Down
}

public struct Damage
{
    public GameObject From;
    public EDamageType DamageType;
    public float DamageForce;
    public int DamageValue;
}
