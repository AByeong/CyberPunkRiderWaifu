using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BossPhase1 : EliteEnemy
{
    [Header("Prefab")]
    public GameObject MissilePrefab;
    public GameObject LaserPrefab;

    [Header("VFX")]
    public ParticleSystem BulletHitVFX;
    public GameObject WarningVFX;

    [SerializeField] private float _hitDelay;
    [SerializeField] private float _granadeRadius;
    [SerializeField] private float _missileRadius;
    [SerializeField] private float _yPosOffset = 1.5f;
    [SerializeField] private float _arcHeight = 10f;


    private Damage _attack0Damage;
    private Damage _attack1Damage;
    private Damage _attack2Damage;

    private Coroutine laserCoroutine;


    protected override void Awake()
    {
        base.Awake();


        _attack0Damage = new Damage()
        {
            DamageType = EDamageType.Airborne,
            DamageValue = 10,
            DamageForce = 2f,
            From = gameObject,
            AirRiseAmount = 0f
        };

        _attack1Damage = new Damage()
        {
            DamageType = EDamageType.Normal,
            DamageValue = 10,
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
            AirRiseAmount = 0f
        };

    }

    // 패턴 1
    public void BustShot()
    {
        Vector3 targetPosition = Target.transform.position;



        StartCoroutine(ShotFire(targetPosition));
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
                _attack0Damage.From = hitVFX.gameObject;
                collider.GetComponent<PlayerHit>().TakeDamage(_attack0Damage);
            }
        }
    }




    GameObject laser;
    // 패턴 2
    public void StartLaser()
    {
        laser = Instantiate(LaserPrefab, transform.position, transform.rotation);
        laser.transform.parent = transform;

        laserCoroutine = StartCoroutine(FireLaser());

    }
    
    public void LaserEnd()
    {
        Destroy(laser);
        // 레이저 종료 (코루틴 종료)
        if (laserCoroutine != null)
        {
            StopCoroutine(laserCoroutine);
            laserCoroutine = null;
        }
    }

    private IEnumerator FireLaser()
    {
        while (true)
        {
            Vector3 origin = transform.position + Vector3.up * _yPosOffset;
            Vector3 dir = transform.forward;

            Vector3 center = origin + dir * (100f / 2f);
            Vector3 halfExtents = new Vector3(5 / 2f, 5 / 2f, 100 / 2f);
            Quaternion rot = Quaternion.LookRotation(dir);

            Collider[] hits = Physics.OverlapBox(center, halfExtents, rot);
            foreach (var col in hits)
            {
                if (col.tag == "Player")
                {
                    col.GetComponent<PlayerHit>()?.TakeDamage(_attack1Damage);
                }
            }

            yield return null;
        }
    }


    // 패턴 3
    public void MissileSwamp()
    {
        Vector3 targetPosition = Target.transform.position;
        Vector3 startPosition = transform.position + Vector3.up * _yPosOffset;
        Vector3 previousPos = startPosition;

        Vector3 midPoint = (startPosition + targetPosition) / 2;
        Vector3 direction = (targetPosition - startPosition).normalized;
        Vector3 upLike = Vector3.Cross(direction, Random.onUnitSphere).normalized;
        Vector3 control = midPoint + upLike * _arcHeight;

        GameObject missile = Instantiate(MissilePrefab);
        missile.transform.position = startPosition;

        DOTween.To(() => 0f, t =>
            {
                Vector3 newPos = CalculateQuadraticBezierPoint(t, startPosition, control, targetPosition);
                missile.transform.position = newPos;

                Vector3 moveDir = (newPos - previousPos).normalized;
                if (moveDir != Vector3.zero)
                    missile.transform.forward = moveDir;

                previousPos = newPos;
            }, 1f, _hitDelay)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                ParticleSystem vfx = Instantiate(BulletHitVFX, missile.transform.position, transform.rotation);
                Destroy(missile);

                Collider[] colliders = Physics.OverlapSphere(targetPosition, _missileRadius);
                foreach (Collider collider in colliders)
                {
                    if (collider.tag == "Player")
                    {
                        _attack2Damage.From = vfx.gameObject;
                        collider.GetComponent<PlayerHit>().TakeDamage(_attack2Damage);
                    }
                }
            });
    }
    
    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // 2차 베지어 공식: (1 - t)^2 * P0 + 2(1 - t)t * P1 + t^2 * P2
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }
}
