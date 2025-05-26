using UnityEngine;

public enum EDamageType
{
    // TODO
    TODO
    ,NoDamageButAir
}

public struct Damage
{
    public GameObject From;
    public EDamageType DamageType;
    public float DamageForce;
    public int DamageValue;
}
