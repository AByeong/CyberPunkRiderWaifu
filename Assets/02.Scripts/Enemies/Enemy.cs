using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public abstract class Enemy : MonoBehaviour, IDamageable
{
    public string MaterialName;
    [SerializeField]
    private EnemyDataSO _enemyData;
    public ObjectPool Pool;

    public const float GRAVITY = 9.8f;

    public bool IsHit { get; set; }
    public bool IsInAir { get; set; }
    public Vector3 VerticalVelocity = new Vector3();

    public Damage TakedDamage => _takedDamage;
    private Damage _takedDamage = new Damage();

    protected Animator _animator;
    protected NavMeshAgent _navMeshAgent;
    protected CharacterController _characterController;
    protected Collider _collider;

    private IStatsProvider _stat;
    // TODO
    // private DropTable _dropTable;

    public GameObject Target { get; set; }
    public EnemyDataSO EnemyData => _enemyData;
    public int CurrentHealthPoint { get; private set; }

    public Animator Animator => _animator;
    public NavMeshAgent NavMeshAgent => _navMeshAgent;
    public Collider Collider => _collider;
    public Transform DamagePopupPosition;
    public GameObject DamagePopup;
    public GameObject WorldSpaceCanvas;

    protected virtual void Awake()
    {
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

        _collider = GetComponent<Collider>();
        if (_collider == null)
        {
            Debug.LogWarning($"{gameObject.name} Collider가 없습니다");
        }

        CurrentHealthPoint = _enemyData.HealthPoint;
        IsHit = false;
        IsInAir = false;
    }

    private async void Start()
    {
        _stat = await StatLoader.LoadFromCSVAsync("EnemyStat.csv");
        _stat = new StatModifierDecorator(_stat, StatType.AttackPower, 20);
    }

    public virtual void Initialize()
    {
        // NavMeshAgent.enabled = true;
        CurrentHealthPoint = EnemyData.HealthPoint;
        IsHit = false;
        IsInAir = false;
    }

    public void PlayHitParticle()
    {
        
    }
    
    
    
    public void TakeDamage(Damage damage)
    {
        IsHit = true;
        CurrentHealthPoint -= damage.DamageValue;
        _takedDamage = damage;
        // Vector3 damagedForceDir = (gameObject.transform.position - damage.From.transform.position).normalized;
        Vector3 worldPos = DamagePopupPosition.position;

        // Canvas 하위로 생성
        GameObject popup = Instantiate(DamagePopup, WorldSpaceCanvas.transform);
        popup.transform.position = worldPos;
        popup.GetComponentInChildren<TypingEffect>().Typing(damage.DamageValue.ToString());
        // 선택: 파괴 또는 애니메이션
        Destroy(popup, 1.5f);
    }

    public List<GameObject> GetDrops() // TODO: List<Item>으로 변경예정
    {
        List<GameObject> drops = new List<GameObject>();
        // TODO
        return drops;
    }
}
