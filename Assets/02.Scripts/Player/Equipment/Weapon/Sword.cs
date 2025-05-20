using System;
using UnityEngine;

public class Sword : MonoBehaviour, IWeapon
{
    public void Attack()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        Debug.Log("Sword");
        
        if (damageable != null)
        {
            Damage damage = new Damage();
            damage.DamageForce = 10.0f; // Weapon Damage Table 있으면 수정
            damage.DamageType = EDamageType.TODO; // 추후 타입 추가
            damage.DamageValue = 100; // Weapon Damage Table 있으면 수정
            damage.From = transform.root.gameObject;
            damageable.TakeDamage(damage);          
        }
    }
}
