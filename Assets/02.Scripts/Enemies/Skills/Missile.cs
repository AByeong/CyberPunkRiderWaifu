using DG.Tweening;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [Header("VFX")]
    public ParticleSystem ExplosionVFX;


    [Header("Parmeters")]
    public float DamageRadius = 4f;
    public float ArcHeight = 10f;
    private Vector3 targetPosition;

    private Damage _damage = new Damage
    {
        DamageType = EDamageType.Airborne,
        DamageValue = 30,
        DamageForce = 4f,
        AirRiseAmount = 0f
    };

    
    public void SetDamage(Damage damage)
    {
        _damage = damage;
        _damage.From = gameObject;
    }

    // public void SetTarget(GameObject target)
    // {
    //     targetPosition = target.transform.position;
    // }

    // public void LaunchMissile(float velocity)
    // {
    //     Vector3 startPosition = transform.position;
    //     Vector3 previousPos = startPosition;

    //     Vector3 midPoint = (startPosition + targetPosition) / 2;
    //     Vector3 direction = (targetPosition - startPosition).normalized;
    //     Vector3 upLike = Vector3.Cross(direction, Random.onUnitSphere).normalized;
    //     Vector3 control = midPoint + upLike * ArcHeight;

    //     DOTween.To(() => 0f, t =>
    //     {
    //         Vector3 newPos = CalculateQuadraticBezierPoint(t, startPosition, control, targetPosition);
    //         transform.position = newPos;

    //         Vector3 moveDir = (newPos - previousPos).normalized;
    //         if (moveDir != Vector3.zero) gameObject.transform.forward = moveDir;

    //         previousPos = newPos;
    //     }, 1f, velocity)
    //     .SetEase(Ease.InOutQuad);
    // }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Boss" || other.tag == "Skill")
        {
            return;
        }

        Instantiate(ExplosionVFX, gameObject.transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(targetPosition, DamageRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Player")
            {
                collider.GetComponent<PlayerHit>().TakeDamage(_damage);
            }

            if(collider.tag == "NoramlEnemy")
            {
                _damage.DamageValue = 0;
                collider.GetComponent<PlayerHit>().TakeDamage(_damage);
            }
        }

        Destroy(gameObject);
    }

    // private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    // {
    //     // 2차 베지어 공식: (1 - t)^2 * P0 + 2(1 - t)t * P1 + t^2 * P2
    //     float u = 1 - t;
    //     return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    // }
}
