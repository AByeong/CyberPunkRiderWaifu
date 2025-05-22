using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
public abstract class Enemy : MonoBehaviour, IDamageable
{

    [SerializeField]
    private EnemyDataSO _enemyData;
    public ObjectPool Pool;

    protected Animator _animator;
    protected BehaviorGraphAgent _behaviorGraphAgent;
    protected BlackboardReference _blackboardRef;
    protected NavMeshAgent _navMeshAgent;

    private IStatsProvider _stat;
    // TODO
    // private DropTable _dropTable;

    protected GameObject _target;
    public EnemyDataSO EnemyData => _enemyData;
    public int CurrentHealthPoint { get; private set; }

    public Animator Animator => _animator;
    public NavMeshAgent NavMeshAgent => _navMeshAgent;
    public BlackboardReference BlackboardRef => _blackboardRef;
    protected virtual void Awake()
    {
        _behaviorGraphAgent = GetComponent<BehaviorGraphAgent>();
        CurrentHealthPoint = _enemyData.HealthPoint;
        
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

    private async void Start()
    {
        _stat = await StatLoader.LoadFromCSVAsync("EnemyStat.csv");

        if (_behaviorGraphAgent != null)
        {
            _blackboardRef = _behaviorGraphAgent.BlackboardReference;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} BehaviorGraphAgent가 없습니다!!");
        }

        _blackboardRef.SetVariableValue("StaggerTime", _enemyData.StaggerTime);
        _blackboardRef.SetVariableValue("AttackCoolDown", _enemyData.AttackCoolDown);
        _blackboardRef.SetVariableValue("AttackDistance", _enemyData.AttackDistance);
        Initialize();

        _stat = new StatModifierDecorator(_stat, StatType.AttackPower, 20);
    }

    public virtual void Initialize()
    {
        _blackboardRef.SetVariableValue("HealthPoint", _enemyData.HealthPoint);
        _blackboardRef.SetVariableValue("EEnemyState", EEnemyState.Idle);
    }

    public void TakeDamage(Damage damage)
    {
        if (_blackboardRef == null)
        {
            Debug.LogError($"{gameObject.name} BlackboardRef가 없습니다!!");
            return;
        }

        Vector3 damagedForceDir = (gameObject.transform.position - damage.From.transform.position).normalized;

        CurrentHealthPoint -= damage.DamageValue;
        Debug.Log("Enemy Hit!");

        _blackboardRef.SetVariableValue("DamageValue", damage.DamageValue);
        _blackboardRef.SetVariableValue("HealthPoint", CurrentHealthPoint);
        _blackboardRef.SetVariableValue("EEnemyState", EEnemyState.Hit);
        _blackboardRef.SetVariableValue("IsHit", true);
    }

    public List<GameObject> GetDrops() // TODO: List<Item>으로 변경예정
    {
        List<GameObject> drops = new List<GameObject>();
        // TODO
        return drops;
    }
}
