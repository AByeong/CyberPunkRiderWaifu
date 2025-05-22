using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
public abstract class Enemy : MonoBehaviour, IDamageable
{

    [SerializeField]
    private EnemyDataSO _enemyData;
    public ObjectPool Pool;

    public const float GRAVITY = 9.8f;

    public bool IsHit { get; set; }
    public bool IsInAir { get; set; }
    public int TakedDamageValue { get; private set; }
    public Vector3 VerticalVelocity = new Vector3();

    protected Animator _animator;
    protected NavMeshAgent _navMeshAgent;
    protected CharacterController _characterController;

    private IStatsProvider _stat;
    // TODO
    // private DropTable _dropTable;

    public GameObject Target { get; set; }
    public EnemyDataSO EnemyData => _enemyData;
    public int CurrentHealthPoint { get; private set; }

    public Animator Animator => _animator;
    public NavMeshAgent NavMeshAgent => _navMeshAgent;

    protected virtual void Awake()
    {
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
        Initialize();

        _stat = new StatModifierDecorator(_stat, StatType.AttackPower, 20);
    }

    public virtual void Initialize()
    {
        CurrentHealthPoint = EnemyData.HealthPoint;
        IsHit = false;
        IsInAir = false;
    }

    public void TakeDamage(Damage damage)
    {
        IsHit = true;
        CurrentHealthPoint -= damage.DamageValue;
        TakedDamageValue = damage.DamageValue;

        Vector3 damagedForceDir = (gameObject.transform.position - damage.From.transform.position).normalized;
    }

    public List<GameObject> GetDrops() // TODO: List<Item>으로 변경예정
    {
        List<GameObject> drops = new List<GameObject>();
        // TODO
        return drops;
    }
}
