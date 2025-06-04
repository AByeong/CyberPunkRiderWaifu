using UnityEngine;

public class EliteEnemy : Enemy, IDamageable
{
    [Header("[Elite Parameters]")]
    public float AttackTimer = 0f;
    public int AttackTypeNumber;

    protected ElliteStateMachine _eliteStateMachine;

    [SerializeField] private int _attackType = 0;
    public int AttackType
    {
        get { return _attackType; }
        set
        {
            if (_attackType != value) // 값이 실제로 변경되었을 때만 실행
            {
                _attackType = value;
                if (_animator != null)
                {
                    _animator.SetInteger("AttackType", _attackType);
                }
                else
                {
                    Debug.LogWarning($"Animator가 {gameObject.name}에 할당되지 않았습니다. AttackType 변경 시 애니메이터 업데이트 불가.");
                }
            }
        }
    }


    public override void TakeDamage(Damage damage)
    {
        base.TakeDamage(damage);
        if (damage.DamageValue != 0) DeliveryManager.Instance.UltimateGaze++;
        SoundManager.Instance.Play(SoundType.Elite_male_Hit);
    }

    protected override void Start()
    {
        base.Start();

        _eliteStateMachine = GetComponent<ElliteStateMachine>();
        if (_eliteStateMachine == null)
        {
            Debug.LogError($"{gameObject.name}의 StateMachine이 없습니다.");
        }

        if (_animator != null)
        {
            _animator.SetInteger("AttackType", _attackType);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}의 Animator가 start 시점에 할당되지 않았습니다. AttackType 초기화 실패 가능.");
        }
    }

    protected virtual void Update()
    {
        if (AttackTimer < EnemyData.AttackCoolDown && !_eliteStateMachine.IsCurrentState<EliteAttackState>())
        {
            AttackTimer += Time.deltaTime;
        }

        if (_animator != null && _navMeshAgent != null && _navMeshAgent.isOnNavMesh)
        {
            _animator.SetFloat("Velocity", _navMeshAgent.velocity.magnitude);
        }
    }
}