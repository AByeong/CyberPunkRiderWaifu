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

        _stat = new StatModifierDecorator(_stat, StatType.AttackPower, 20);
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
