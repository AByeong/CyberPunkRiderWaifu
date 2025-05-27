using UnityEngine;

public class NormalEnemy : Enemy
{
    public Collider WeaponCollider;

    protected override void Awake()
    {
        base.Awake();
        WeaponCollider.enabled = false;
    }

    void Update()
    {
        _animator.SetFloat("Velocity", _navMeshAgent.velocity.magnitude);
    }
}
