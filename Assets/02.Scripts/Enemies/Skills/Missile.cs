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

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Boss" || other.tag == "Skill")
        {
            return;
        }

        Instantiate(ExplosionVFX, gameObject.transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, DamageRadius);
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
