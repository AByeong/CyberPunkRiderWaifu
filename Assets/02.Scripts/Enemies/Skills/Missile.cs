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
                collider.GetComponent<Enemy>().TakeDamage(_damage);
            }
        }

        Destroy(gameObject);
    }
}
