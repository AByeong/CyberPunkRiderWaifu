using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;


public abstract class Enemy : MonoBehaviour, IDamageable
{
    public EnemyDataSO EnemyData => _enemyData;
    public int CurrentHealthPoint => _currentHealthPoint;

    public Animator Animator => _animator;
    public NavMeshAgent NavMeshAgent => _navMeshAgent;
    public BlackboardReference BlackboardRef => _blackboardRef;

    [SerializeField]
    private EnemyDataSO _enemyData;
    private int _currentHealthPoint;

    protected Animator _animator;
    protected NavMeshAgent _navMeshAgent;
    protected BehaviorGraphAgent _behaviorGraphAgent;
    protected BlackboardReference _blackboardRef;

    // TODO
    // private DropTable _dropTable;

    protected GameObject _target;
    public ObjectPool Pool;
    protected virtual void Awake()
    {
        _behaviorGraphAgent = GetComponent<BehaviorGraphAgent>();
        _currentHealthPoint = _enemyData.HealthPoint;
    }

    private void Start()
    {
        if (_behaviorGraphAgent != null)
        {
            _blackboardRef = _behaviorGraphAgent.BlackboardReference;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} BehaviorGraphAgent가 없습니다!!");
        }

        _blackboardRef.SetVariableValue("StaggerTime", _enemyData.StaggerTime);
    }

    public void TakeDamage(Damage damage)
    {
        if (_blackboardRef == null)
        {
            Debug.LogError($"{gameObject.name} BlackboardRef가 없습니다!!");
            return;
        }

        Vector3 damagedForceDir = (gameObject.transform.position - damage.From.transform.position).normalized;

        _currentHealthPoint -= damage.DamageValue;

        _blackboardRef.SetVariableValue("EEnemyState", EEnemyState.Hit);
        _blackboardRef.SetVariableValue("IsHit", true);
    }

    public void MinusHealthPoint(int amount)
    {
        _currentHealthPoint -= amount;
    }

    public List<GameObject> GetDrops() // TODO: List<Item>으로 변경예정
    {
        List<GameObject> drops = new List<GameObject>();
        // TODO
        return drops;
    }
}
