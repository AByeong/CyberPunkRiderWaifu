using System.Collections;
using UnityEngine;

public class BossPhase1 : EliteEnemy
{
    [Header("Prefab")]
    public Missile MissilePrefab;
    public Laser LaserPrefab;

    [Header("VFX")]
    public ParticleSystem BulletHitVFX;
    public GameObject WarningVFX;

    [Header ("Parameters")]
    [SerializeField] private float _hitDelay;
    [SerializeField] private float _missileHitTime;
    [SerializeField] private float _granadeRadius;
    [SerializeField] private float _yPosOffset = 1.5f;


    private Damage _attack0Damage;
    private Damage _attack1Damage;
    private Damage _attack2Damage;
    private float rotateSpeed = 30f;

    

    protected override void Awake()
    {
        base.Awake();

        _attack0Damage = new Damage()
        {
            DamageType = EDamageType.Normal,
            DamageValue = 10,
            DamageForce = 2f,
            From = gameObject,
            AirRiseAmount = 0f
        };

        _attack1Damage = new Damage()
        {
            DamageType = EDamageType.Normal,
            DamageValue = 5,
            DamageForce = 1f,
            From = gameObject,
            AirRiseAmount = 0f
        };

        _attack2Damage = new Damage()
        {
            DamageType = EDamageType.Airborne,
            DamageValue = 30,
            DamageForce = 4f,
            From = gameObject,
            AirRiseAmount = 2f
        };

    }

    protected override void Update()
    {
        base.Update();

        if (_eliteStateMachine.IsCurrentState<EliteAttackState>())
        {
            Vector3 targetPos = Target.transform.position;
            targetPos.y = transform.position.y;
            Quaternion targetRotation = Quaternion.LookRotation(targetPos - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }

    // 패턴 1
    private Damage _enemyDamage;
    public void BustShot()
    {
        StartCoroutine(ShotFire(Target.transform.position));
    }

    private IEnumerator ShotFire(Vector3 targetPosition)
    {
        GameObject warningVFX = Instantiate(WarningVFX);
        WarningVFX.transform.position = targetPosition;

        yield return new WaitForSeconds(_hitDelay);
        Destroy(warningVFX);

        ParticleSystem hitVFX = Instantiate(BulletHitVFX, targetPosition, Quaternion.identity);
        hitVFX.Play();

        Collider[] colliders = Physics.OverlapSphere(targetPosition, _granadeRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Player")
            {
                collider.GetComponent<PlayerHit>().TakeDamage(_attack0Damage);
            }

            if(collider.tag == "NormalEnemy")
            {
                _enemyDamage = _attack0Damage;
                _enemyDamage.DamageValue = 0;
                _enemyDamage.DamageType = EDamageType.Airborne;
                collider.GetComponent<Enemy>().TakeDamage(_enemyDamage);
            }
        }
    }


    // 패턴 2
    public void StartLaser()
    {
        Laser laser = Instantiate(LaserPrefab, transform.position, transform.rotation);
        laser.transform.parent = transform;
        laser.SetDamage(_attack1Damage);
    }


    // 패턴 3
    public void MissileSwamp()
    {
        Vector3 startPosition = transform.position + Vector3.up * _yPosOffset + Vector3.back * 0.2f;

        Missile missile = Instantiate(MissilePrefab, startPosition, transform.rotation);
        missile.SetTarget(Target);
        missile.SetDamage(_attack2Damage);
        missile.LaunchMissile(_missileHitTime);
    }
}
