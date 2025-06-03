using UnityEngine;

public class EnergyBall : MonoBehaviour
{
    [Header("[DamageParameters]")]
    public EDamageType InDamageType;
    public int InDamageValue;
    public float InDamageForce;
    public float InAirRiseAmount;

    private Damage _playerDamage;
    private Damage _enemyDamage;

    private void Awake()
    {
        _playerDamage = new Damage()
        {
            DamageType = InDamageType,
            DamageValue = InDamageValue,
            DamageForce = InDamageForce,
            AirRiseAmount = InAirRiseAmount,
            From = gameObject
        };

        _enemyDamage = new Damage()
        {
            DamageType = EDamageType.Airborne,
            DamageValue = 0,
            DamageForce = InDamageForce,
            AirRiseAmount = InAirRiseAmount,
            From = gameObject
        };
        if (InAirRiseAmount == 0)
        {
            _enemyDamage.AirRiseAmount = 1f;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            damageable.TakeDamage(_playerDamage);
        }

        if (other.tag == "NormalEnemy")
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            damageable.TakeDamage(_enemyDamage);
        }
    }
}
