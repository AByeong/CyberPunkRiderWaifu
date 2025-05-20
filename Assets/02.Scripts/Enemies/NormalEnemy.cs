using UnityEngine;
using UnityEngine.AI;

public class NormalEnemy : Enemy
{
    protected override void Awake()
    {
        base.Awake();

        _navMeshAgent = GetComponent<NavMeshAgent>();
        if (_navMeshAgent == null)
        {
            Debug.LogWarning($"{gameObject.name} NavMeshAgent가 없습니다");
        }

        _animator = GetComponentInChildren<Animator>();
        if (_animator == null)
        {
            Debug.LogWarning($"{gameObject.name} Animator가 없습니다");
        }
    }

    void Update()
    {
        _animator.SetFloat("Velocity", _navMeshAgent.velocity.magnitude);
    }

}
