using UnityEngine;

public class Laser : MonoBehaviour
{
    private Vector3 center;
    private Vector3 halfExtents;
    private Quaternion rot;

    private Damage damage;


    private void Start()
    {
        Vector3 origin = transform.position;
        Vector3 dir = transform.forward;

        center = origin + dir * (100f / 2f);
        halfExtents = new Vector3(5 / 2f, 5 / 2f, 100 / 2f);
        rot = Quaternion.LookRotation(dir);

        damage = new Damage()
        {
            DamageType = EDamageType.Normal,
            DamageValue = 10,
            DamageForce = 1f,
            From = gameObject,
            AirRiseAmount = 0f
        };
    }

    private void Update()
    {
         Collider[] hits = Physics.OverlapBox(center, halfExtents, rot);
        foreach (var col in hits)
        {
            if (col.tag == "Player")
            {
                col.GetComponent<PlayerHit>()?.TakeDamage(damage);
            }
        }
    }
}
