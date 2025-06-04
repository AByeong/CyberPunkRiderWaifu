using System.Threading;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [Header("VFX")]
    public ParticleSystem ExplosionVFX;


    [Header("Parmeters")]
    public BossPhase1 Owner;
    public float DamageRadius = 4f;
    public float ArcHeight = 10f;
    private Vector3 targetPosition;

    private float _duration = 1f;
    private float _timer;


    private Damage _damage = new Damage
    {
        DamageType = EDamageType.Airborne,
        DamageValue = 30,
        DamageForce = 4f,
        AirRiseAmount = 2f
    };

    private Damage _enemyDamage;


    public void SetDamage(Damage damage)
    {
        _damage = damage;
        _damage.From = gameObject;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _duration)
        {
            Attack(transform.position, DamageRadius, _damage, true);
            Instantiate(ExplosionVFX, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Boss" || other.tag == "Skill")
        {
            return;
        }

        Attack(transform.position, DamageRadius, _damage, true);
        Instantiate(ExplosionVFX, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
    
    public virtual void Attack(Vector3 attackPos, float attackRadius, Damage damage, bool isEnemyHit = false)
    {
        Collider[] detectedColliders = Physics.OverlapSphere(attackPos, attackRadius);
        foreach (Collider hitCollider in detectedColliders)
        {

            if (hitCollider.tag == "NormalEnemy" && isEnemyHit)
            {
                IDamageable damageable = hitCollider.GetComponent<IDamageable>();
                {
                    _enemyDamage = damage;
                    _enemyDamage.DamageValue = 0;
                    
                    if (_enemyDamage.DamageType != EDamageType.Airborne)
                    {
                        _enemyDamage.DamageType = EDamageType.Airborne;
                    }

                    if (_enemyDamage.AirRiseAmount == 0)
                    {
                        _enemyDamage.AirRiseAmount = 2f;
                    }

                    damageable.TakeDamage(_enemyDamage);
                }

                continue;
            }

            if (hitCollider.tag == "Player")
            {
                IDamageable damageable = hitCollider.GetComponent<IDamageable>();
                {
                    damageable.TakeDamage(damage);
                }
            }
        }
    }
}
