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
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
    }
    private PlayerController _player => GameManager.Instance.player;
    public int Index;

    private bool IsCriticalHit()
    {
        float rand = Random.value; // 0.0f ~ 1.0f 사이의 무작위 값
        return rand < _player.CritChance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) return;
        if (other.TryGetComponent(out IDamageable damageable))
        {
            Damage damage = new Damage();
            damage.DamageForce = 1f; // Weapon Damage Table 있으면 수정
            damage.DamageType = SkillManager.Instance.DataList.SkillData[Index].DamageType; // 추후 타입 추가
            if (IsCriticalHit())
            {
                damage.DamageValue = (int)(_player.AttackPower * _player.CritDamage * SkillManager.Instance.DataList.SkillData[Index].SkillDamage);
                damage.DamageCriType = EDamageCriType.Critical;
            }
            else
            {
                damage.DamageValue = (int)(_player.AttackPower * SkillManager.Instance.DataList.SkillData[Index].SkillDamage);
            }
            damage.From = transform.root.gameObject;
            damageable.TakeDamage(damage);         
        }
    }
}
