using UnityEngine;

public class EliteEnemy : Enemy, IDamageable
{
    // 기존 public int AttackType = 0; 대신 아래와 같이 변경
    [SerializeField] // 인스펙터에서 초기값을 설정하고 보기 위함
    private int _attackType = 0; // 실제 AttackType 값을 저장할 private 백킹 필드

    public float OriginalValue = 0.5f;
    public bool IsAttacking = false;
    public float AttackTimer = 0f;

    // public int AttackTypeNumber = 2;
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


    public void TakeDamage(Damage damage)
    {
        SoundManager.Instance.Play(SoundType.Elite_male_Hit);
        
        base.TakeDamage(damage);
    }

    protected ElliteStateMachine _eliteStateMachine;

    private void Start()
    {
        _eliteStateMachine = GetComponent<ElliteStateMachine>();
        if (_eliteStateMachine == null)
        {
           Debug.LogWarning($"{gameObject.name}의 StateMachine이 없습니다.");
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
        if (_animator != null && _navMeshAgent != null && _navMeshAgent.isOnNavMesh) // isOnNavMesh 추가
        {
            _animator.SetFloat("Velocity",  _navMeshAgent.velocity.magnitude);
        }
    }
}