using JY;
using UnityEngine;
public class Sword : MonoBehaviour, IWeapon
{
    private PlayerController _playerController;

    private void Start()
    {
        _playerController = GameManager.Instance.player.GetComponent<PlayerController>();
    }
    public void Attack()
    {
        
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
            damage.DamageType = EDamageType.Normal; // 추후 타입 추가
            damage.DamageValue = (int)_playerController.AttackPower; // Weapon Damage Table 있으면 수정
            damage.From = transform.root.gameObject;
            damageable.TakeDamage(damage);          
        }
    }
}
