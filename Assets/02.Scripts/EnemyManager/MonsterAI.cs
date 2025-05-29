using UnityEngine;
using UnityEngine.AI;

// AITypes.cs 또는 공용 파일에 정의된 AITier enum을 사용한다고 가정합니다.
// public enum AITier { Tier1_ActiveFormation, Tier2_Approaching, Tier3_Background }

public abstract class MonsterAI : MonoBehaviour
{
    [Header("Runtime Status")]
    public AITier CurrentTier = AITier.Tier3_Background;

    public Enemy Enemy; // Enemy 스크립트에 EnemyData 및 AttackDistance가 있다고 가정
    protected NavMeshAgent navMeshAgent;
    protected Transform playerTransform;
    protected FormationManager formationManager; // 파생 클래스에서 사용 가능
    protected AIManager aiManager;

    [Header("Common Speeds & Config")]
    public float DefaultTier1Speed = 3.5f;
    public float DefaultTier2Speed = 7f;
    public float DefaultTier1Acceleration = 8f;
    public float DefaultTier2Acceleration = 5f;

    [Header("Tier 2 Behavior")]
    protected const float Tier2_ApproachStoppingDistance = 8f;

    private Renderer monsterRenderer; // 필요시 사용 (예: 시야 검사, 디버깅)

    [Header("Performance")]
    public float logicUpdateInterval = 0.2f; // AIManager에서 읽기 위해 public
    public float nextIndividualLogicUpdateTime = 0f; // AIManager에서 읽고 쓰기 위해 public으로 변경

    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError($"[{this.GetType().Name}] NavMeshAgent component not found on {gameObject.name}, adding one.", this.gameObject);
            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        }
        navMeshAgent.updateRotation = true; // NavMeshAgent가 회전을 직접 제어

        monsterRenderer = GetComponentInChildren<Renderer>();
        if (monsterRenderer == null)
        {
            Debug.LogWarning($"[{this.GetType().Name}] Renderer component not found on child of {gameObject.name}", this.gameObject);
        }
    }

    protected virtual void OnDisable()
    {
        if (aiManager != null)
        {
            aiManager.UnregisterMonster(this); // AIManager가 MonsterAI 타입을 받을 수 있어야 함
        }
        HandleOnDisable(); // 파생 클래스에서 포메이션 슬롯 해제 등을 처리
        if (navMeshAgent != null && navMeshAgent.isOnNavMesh && navMeshAgent.enabled)
        {
            navMeshAgent.isStopped = true;
        }
    }

    // 파생 클래스에서 재정의하여 포메이션 슬롯 등을 해제
    protected virtual void HandleOnDisable() { }

    public virtual void Initialize(Transform player, FormationManager fm, AIManager am, ObjectPool pool)
    {
        if (navMeshAgent == null) navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent.enabled) navMeshAgent.enabled = false; // 에이전트 리셋을 위해 잠시 비활성화
        navMeshAgent.enabled = true;

        playerTransform = player;
        formationManager = fm;
        aiManager = am;
        // 초기 업데이트 시간 분산을 위해 Random.Range 사용. AIManager가 이 값을 읽고 첫 업데이트 후 다시 설정.
        this.nextIndividualLogicUpdateTime = Time.time + Random.Range(0f, logicUpdateInterval);

        if (Enemy != null)
        {
            Enemy.Target = player.gameObject;
            // if (pool != null && Enemy.GetType().GetProperty("Pool") != null) // Enemy에 Pool 속성이 있는지 확인
            // {
            //      // Enemy.Pool = pool; // Enemy 클래스에 Pool 멤버가 있다면 사용
            // }
        }
        else
        {
            Debug.LogError($"[{this.GetType().Name}] Enemy component is not assigned on {gameObject.name}", this.gameObject);
        }
    }

    // AIManager에서 monster.logicUpdateInterval을 직접 사용하므로 이 메서드가 외부에서 꼭 필요하진 않을 수 있음.
    // MonsterAI 내부 및 파생 클래스에서 일관된 방식으로 logicUpdateInterval을 가져오고 싶을 때 유용.
    protected virtual float GetLogicUpdateInterval() => this.logicUpdateInterval;


    public virtual void SetAITier(AITier newTier, bool forceUpdate = false)
    {
        if (CurrentTier == newTier && !forceUpdate) return;

        AITier previousTier = CurrentTier;
        CurrentTier = newTier;

        if (previousTier == AITier.Tier1_ActiveFormation && newTier != AITier.Tier1_ActiveFormation)
        {
            OnExitTier1(); // Tier1에서 나갈 때 파생 클래스가 처리할 내용 (예: 포메이션 슬롯 해제)
        }

        navMeshAgent.isStopped = false; // 기본적으로 이동 상태로 설정

        switch (CurrentTier)
        {
            case AITier.Tier1_ActiveFormation:
                navMeshAgent.speed = GetTier1Speed();
                navMeshAgent.acceleration = GetTier1Acceleration();
                navMeshAgent.autoRepath = true;
                ConfigureNavMeshAgentForTier1(); // stoppingDistance 등 Tier1 특화 설정
                this.logicUpdateInterval = 0.1f; // Tier1 인터벌 설정 (예시)
                break;

            case AITier.Tier2_Approaching:
                navMeshAgent.speed = GetTier2Speed();
                navMeshAgent.acceleration = DefaultTier2Acceleration;
                navMeshAgent.stoppingDistance = Tier2_ApproachStoppingDistance;
                navMeshAgent.autoRepath = true;
                this.logicUpdateInterval = 0.3f; // Tier2 인터벌 설정 (예시)
                break;

            case AITier.Tier3_Background:
                if (navMeshAgent.enabled && navMeshAgent.isOnNavMesh)
                {
                    navMeshAgent.isStopped = true;
                }
                this.logicUpdateInterval = 1.0f; // Tier3 인터벌 설정 (예시)
                break;
        }
        // Tier 변경 시 다음 업데이트 시간을 현재 시간 + 새 인터벌의 랜덤 값으로 즉시 재설정하여 반응성 향상 및 몰림 방지
        this.nextIndividualLogicUpdateTime = Time.time + Random.Range(0f, this.logicUpdateInterval);
    }

    // 파생 클래스에서 재정의하여 Tier1 관련 설정을 커스터마이징
    protected virtual float GetTier1Speed() => DefaultTier1Speed;
    protected virtual float GetTier1Acceleration() => DefaultTier1Acceleration;
    protected virtual float GetTier2Speed() => DefaultTier2Speed;
    protected abstract void ConfigureNavMeshAgentForTier1(); // Tier1 NavMeshAgent 설정 (stoppingDistance 등)
    protected abstract void OnExitTier1(); // Tier1 상태를 벗어날 때 호출

    public void ManagedUpdateLogic()
    {
        if (playerTransform == null || !gameObject.activeInHierarchy || Enemy == null || (Enemy.EnemyData == null))
        {
            if (navMeshAgent != null && navMeshAgent.enabled && navMeshAgent.isOnNavMesh && !navMeshAgent.isStopped)
            {
                navMeshAgent.isStopped = true; // 필수 조건 미충족 시 정지
            }
            return;
        }

        // Tier3이 아니고, 에이전트가 멈춰있다면 다시 활성화 (Tier 변경 직후 isStopped가 true일 수 있음)
        if (CurrentTier != AITier.Tier3_Background && navMeshAgent.enabled && navMeshAgent.isOnNavMesh && navMeshAgent.isStopped)
        {
            navMeshAgent.isStopped = false;
        }


        switch (CurrentTier)
        {
            case AITier.Tier1_ActiveFormation: UpdateTier1Behavior(); break;
            case AITier.Tier2_Approaching: UpdateTier2Behavior(); break;
            case AITier.Tier3_Background: UpdateTier3Behavior(); break;
        }
    }

    protected abstract void UpdateTier1Behavior();

    protected virtual void UpdateTier2Behavior()
    {
        if (playerTransform == null) return;

        if (navMeshAgent.enabled && navMeshAgent.isOnNavMesh)
        {
            // SetAITier에서 stoppingDistance가 이미 설정되었으므로 여기서는 목적지만 갱신
            if (navMeshAgent.destination != playerTransform.position)
            {
                navMeshAgent.SetDestination(playerTransform.position);
            }
        }
    }

    protected virtual void UpdateTier3Behavior()
    {
        // SetAITier에서 이미 isStopped = true로 설정됨. 추가 로직이 필요하면 여기에 작성
        if (navMeshAgent.enabled && navMeshAgent.isOnNavMesh && !navMeshAgent.isStopped)
        {
             navMeshAgent.isStopped = true;
        }
    }

    /// <summary>
    /// FormationManager가 몬스터에게 새로운 포메이션 슬롯 위치를 알릴 때 호출합니다.
    /// 포메이션을 사용하는 파생 클래스(현재 NormalMonsterAI)에서 이 메서드를 재정의(override)하여
    /// currentFormationSlot 값을 업데이트하고, 필요시 NavMeshAgent의 목적지를 설정합니다.
    /// </summary>
    /// <param name="newWorldPosition">새로운 월드 포메이션 슬롯 위치. 슬롯을 잃었음을 의미할 경우 Vector3.positiveInfinity와 같은 특수 값을 사용할 수 있습니다.</param>
    public virtual void NotifyNewFormationSlotPosition(Vector3 newWorldPosition)
    {
        // 기본 클래스에서는 특별한 동작을 하지 않습니다.
        // 포메이션을 사용하는 파생 클래스가 이 메서드를 override 하여 실제 로직을 구현합니다.
    }
}