using System;
using JY;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class VFXAttack : MonoBehaviour
{

    public void Reset()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
    }
    private PlayerController _player => GameManager.Instance.player;
    public int Index;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) return;
        if (other.TryGetComponent(out IDamageable damageable))
        {
            Damage damage = new Damage();
            damage.DamageForce = 1f; // Weapon Damage Table 있으면 수정
            damage.DamageType = EDamageType.Normal; // 추후 타입 추가
            damage.DamageValue = (int)(_player.AttackPower * SkillManager.Instance.DataList.SkillData[Index].SkillDamage);
            damage.From = transform.root.gameObject;
            damageable.TakeDamage(damage);         
        }
    }
}
