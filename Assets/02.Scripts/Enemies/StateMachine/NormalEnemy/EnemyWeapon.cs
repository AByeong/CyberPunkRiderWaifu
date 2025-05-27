using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public Enemy Owner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Damage damage = new Damage()
            {
                From = gameObject,
                DamageType = EDamageType.TODO,
                DamageForce = Owner.EnemyData.AttackForce,
                DamageValue = Owner.EnemyData.Damage
            };
            other.GetComponent<IDamageable>().TakeDamage(damage);
        }
    }
}
