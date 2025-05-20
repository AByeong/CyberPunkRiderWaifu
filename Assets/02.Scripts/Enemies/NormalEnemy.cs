using UnityEngine;
using UnityEngine.AI;

public class NormalEnemy : Enemy, IDamageable
{
    private EEnemyType _enemyType = EEnemyType.Normal;
    private float _aggroRange;
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;

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
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TakeDamage(__testDamage__);
            Debug.Log("Q");
        }
    }

}
