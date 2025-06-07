using JY;
using UnityEngine;
public class Sword : MonoBehaviour, IWeapon
{
    private PlayerController _playerController;
    // private EDamageType _damageType;
    private void Start()
    {
        _playerController = GameManager.Instance.player.GetComponent<PlayerController>();
    }
    public void Attack()
    {
        
    }
    private bool IsCriticalHit()
    {
        float rand = Random.value; // 0.0f ~ 1.0f 사이의 무작위 값
        return rand < _playerController.CritChance;
    }
    private void OnTriggerEnter(Collider collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        // Debug.Log("Sword");
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            return;
        }
        if (damageable != null)
        {
            Damage damage = new Damage();
            damage.DamageForce = 1f; // Weapon Damage Table 있으면 수정
            damage.DamageType = _playerController.DamageType; // 추후 타입 추가
            if(IsCriticalHit())
            {
                damage.DamageValue = (int)(_playerController.AttackPower * _playerController.CritDamage); // Weapon Damage Table 있으면 수정
                damage.DamageCriType = EDamageCriType.Critical;
            }
            else
            {
                damage.DamageValue = (int)_playerController.AttackPower; // Weapon Damage Table 있으면 수정
                damage.DamageCriType = EDamageCriType.Normal;
            }
            damage.From = transform.root.gameObject;
            damageable.TakeDamage(damage);          
        }
    }
}
