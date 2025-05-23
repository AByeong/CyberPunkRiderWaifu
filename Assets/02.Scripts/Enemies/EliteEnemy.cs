using UnityEngine;
using UnityEngine.AI;

public class EliteEnemy : Enemy, IDamageable
{
    // 기존 public int AttackType = 0; 대신 아래와 같이 변경
    [SerializeField] // 인스펙터에서 초기값을 설정하고 보기 위함
    private int _attackType = 0; // 실제 AttackType 값을 저장할 private 백킹 필드

    public int AttackTypeNumber = 2;
    public int AttackType
    {
        get { return _attackType; }
        set
        {
            if (_attackType != value) // 값이 실제로 변경되었을 때만 실행
            {
                _attackType = value;
                if (EliteAnimator != null)
                {
                    EliteAnimator.SetInteger("AttackType", _attackType);
                }
                else
                {
                    Debug.LogWarning($"EliteAnimator가 {gameObject.name}에 할당되지 않았습니다. AttackType 변경 시 애니메이터 업데이트 불가.");
                }
            }
        }
    }

    public Animator EliteAnimator; // 인스펙터에서 할당하거나 Awake에서 찾아야 함
    
    private NavMeshAgent _navMeshAgent;
    private Animator _animator; // 일반적인 이동 애니메이션 등을 위한 Animator

   
    protected override void Awake()
    {
       
        if (EliteAnimator != null)
        {
            EliteAnimator.SetInteger("AttackType", _attackType);
        }
        else
        {
          
            Debug.LogWarning($"{gameObject.name}의 EliteAnimator가 Awake 시점에 할당되지 않았습니다. AttackType 초기화 실패 가능.");
        }

        base.Awake(); // Enemy 클래스의 Awake 실행

        _navMeshAgent = GetComponent<NavMeshAgent>();
        if (_navMeshAgent == null)
        {
            Debug.LogWarning($"{gameObject.name} NavMeshAgent가 없습니다");
        }

        _animator = GetComponentInChildren<Animator>(); // 일반 애니메이션(속도 등)을 위한 Animator
        if (_animator == null)
        {
            Debug.LogWarning($"{gameObject.name} Animator가 없습니다");
        }
        
    }

    void Update()
    {
        if (_animator != null && _navMeshAgent != null && _navMeshAgent.isOnNavMesh) // isOnNavMesh 추가
        {
            _animator.SetFloat("Velocity", _navMeshAgent.velocity.magnitude);
        }
    }

   
}