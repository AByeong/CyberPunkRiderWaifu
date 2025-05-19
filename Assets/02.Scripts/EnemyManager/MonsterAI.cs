using UnityEngine;
using UnityEngine.AI;

public enum AITier
{
    Tier1_ActiveFormation,
    Tier2_Approaching,
    Tier3_Background
}


// AITier enum은 파일 상단 또는 별도 파일에 정의되어 있다고 가정
// public enum AITier { Tier1_ActiveFormation, Tier2_Approaching, Tier3_Background }

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAI : MonoBehaviour
{
    public AITier CurrentTier { get; private set; } = AITier.Tier3_Background;

    private NavMeshAgent navMeshAgent;
    private Transform playerTransform;
    private FormationManager formationManager;
    private AIManager aiManager;

    [Header("Tier 1 Behavior (Active Formation & Retreat)")]
    public float minPlayerDistance = 4.0f;
    public float desiredPlayerDistance = 6.0f;
    private const float Tier1_SlotReachedThreshold = 1.5f;
    private const float Tier1_LookAtPlayerSpeed = 5f; // 플레이어 바라보기 속도는 유지
    private bool isRetreating = false;
    private Vector3? currentFormationSlot = null;

    [Header("Tier 2 Behavior (Approaching)")]
    private const float Tier2_ApproachStoppingDistance = 8f;
    private const float Tier2_LookAtPlayerSpeed = 4f; // 플레이어 바라보기 속도는 유지

    [Header("Tier 3 Behavior")]
    private const float Tier3_LookAtPlayerSpeed = 3f; // 플레이어 바라보기 속도는 유지
    public float tier3LookAtPlayerMaxDistance = 10f;
    private float tier3LookAtPlayerMaxDistanceSqr;

    // 색상 관련 필드 제거 (GPU Instancing을 위해 머티리얼 공유)
    // public Color tier1Color = Color.red;
    // public Color tier2Color = Color.yellow;
    // public Color tier3Color = Color.blue;
    // public Color defaultColor = Color.gray;
    private Renderer monsterRenderer; // Renderer 참조는 여전히 유용할 수 있음 (예: 활성화/비활성화)

    [Header("Performance")]
    public float logicUpdateInterval = 0.2f;
    public float nextIndividualLogicUpdateTime = 0f;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false; // 수동 회전(플레이어 바라보기 등)을 위해 필요

        monsterRenderer = GetComponentInChildren<Renderer>(); // Renderer 컴포넌트 가져오기
        if (monsterRenderer == null)
        {
            Debug.LogWarning("MonsterAI: Renderer component not found on " + gameObject.name, this.gameObject);
        }
        // Awake에서 monsterRenderer.material.color 설정하는 부분 제거

        tier3LookAtPlayerMaxDistanceSqr = tier3LookAtPlayerMaxDistance * tier3LookAtPlayerMaxDistance;
    }

    void OnDisable()
    {
        if (aiManager != null) aiManager.UnregisterMonster(this);
        if (currentFormationSlot.HasValue && formationManager != null)
        {
            formationManager.ReleaseFormationSlot(this);
            currentFormationSlot = null;
        }
        isRetreating = false;
        if (navMeshAgent != null && navMeshAgent.enabled && navMeshAgent.isOnNavMesh) navMeshAgent.isStopped = true;
    }

    public void Initialize(Transform player, FormationManager fm, AIManager am)
    {
        playerTransform = player;
        formationManager = fm;
        aiManager = am;
        nextIndividualLogicUpdateTime = Time.time + Random.Range(0, logicUpdateInterval);
    }

    public void SetAITier(AITier newTier, bool forceUpdate = false)
    {
        if (CurrentTier == newTier && !forceUpdate) return;
        AITier previousTier = CurrentTier;
        CurrentTier = newTier;

        if (previousTier == AITier.Tier1_ActiveFormation && newTier != AITier.Tier1_ActiveFormation)
        {
            if (currentFormationSlot.HasValue && formationManager != null)
            {
                formationManager.ReleaseFormationSlot(this);
                currentFormationSlot = null;
            }
        }
        isRetreating = false;

        // AI Tier에 따른 색상 변경 로직 제거 (GPU Instancing을 위해)

        switch (CurrentTier)
        {
            case AITier.Tier1_ActiveFormation:
                navMeshAgent.enabled = true; navMeshAgent.isStopped = false;
                navMeshAgent.speed = 3.5f; navMeshAgent.acceleration = 8f; navMeshAgent.autoRepath = true;
                logicUpdateInterval = 0.1f;
                break;
            case AITier.Tier2_Approaching:
                navMeshAgent.enabled = true; navMeshAgent.isStopped = false;
                navMeshAgent.speed = 2.8f; navMeshAgent.acceleration = 5f;
                navMeshAgent.stoppingDistance = Tier2_ApproachStoppingDistance; navMeshAgent.autoRepath = true;
                logicUpdateInterval = 0.3f;
                break;
            case AITier.Tier3_Background:
                if (navMeshAgent.enabled && navMeshAgent.isOnNavMesh) navMeshAgent.isStopped = true;
                logicUpdateInterval = 1.0f;
                break;
        }
    }

    private void ExecuteLookAtPlayerLogic(float lookSpeed)
    {
        if (playerTransform == null) return;
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        directionToPlayer.y = 0;
        if (directionToPlayer.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);
        }
    }

    public void ManagedUpdateLogic()
    {
        if (playerTransform == null || !gameObject.activeInHierarchy) return;

        switch (CurrentTier)
        {
            case AITier.Tier1_ActiveFormation: UpdateTier1Behavior(); break;
            case AITier.Tier2_Approaching: UpdateTier2Behavior(); break;
            case AITier.Tier3_Background: UpdateTier3Behavior(); break;
        }

        bool shouldLook = true;
        if (CurrentTier == AITier.Tier3_Background)
        {
            if (playerTransform == null || (playerTransform.position - transform.position).sqrMagnitude > tier3LookAtPlayerMaxDistanceSqr)
            {
                shouldLook = false;
            }
        }

        if (shouldLook)
        {
            float currentLookSpeed;
            switch (CurrentTier)
            {
                case AITier.Tier1_ActiveFormation: currentLookSpeed = Tier1_LookAtPlayerSpeed; break;
                case AITier.Tier2_Approaching: currentLookSpeed = Tier2_LookAtPlayerSpeed; break;
                case AITier.Tier3_Background: currentLookSpeed = Tier3_LookAtPlayerSpeed; break;
                default: currentLookSpeed = Tier1_LookAtPlayerSpeed; break;
            }
            ExecuteLookAtPlayerLogic(currentLookSpeed);
        }
    }

    void UpdateTier1Behavior()
    {
        if (!navMeshAgent.enabled) navMeshAgent.enabled = true;
        if (navMeshAgent.isStopped) navMeshAgent.isStopped = false;

        float sqrDistToPlayer = (playerTransform.position - transform.position).sqrMagnitude;
        float minPlayerDistSqr = minPlayerDistance * minPlayerDistance;

        if (sqrDistToPlayer < minPlayerDistSqr && !isRetreating)
        {
            isRetreating = true;
        }

        if (isRetreating)
        {
            Vector3 directionFromPlayer = (transform.position - playerTransform.position).normalized;
            Vector3 retreatTargetPos = playerTransform.position + directionFromPlayer * desiredPlayerDistance;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(retreatTargetPos, out hit, 3.0f, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(hit.position);
                navMeshAgent.stoppingDistance = 0.2f;
            }
            else
            {
                Vector3 emergencyRetreatPos = transform.position + directionFromPlayer * 2.0f;
                if (NavMesh.SamplePosition(emergencyRetreatPos, out hit, 1.0f, NavMesh.AllAreas))
                    navMeshAgent.SetDestination(hit.position);
            }

            float desiredDistSqr = desiredPlayerDistance * desiredPlayerDistance;
            // 후퇴 종료 조건 단순화 또는 이전 로직 유지
            if (sqrDistToPlayer > desiredDistSqr * 0.9f) // 충분히 멀어졌다고 판단
            {
                 // 그리고 NavMeshAgent가 현재 경로를 거의 완료했는지 확인 (선택적)
                if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + 0.5f)
                {
                    isRetreating = false;
                    navMeshAgent.ResetPath(); // 다음 행동을 위해 경로 초기화
                }
            }
            return;
        }

        navMeshAgent.stoppingDistance = Tier1_SlotReachedThreshold * 0.8f;
        bool needsNewSlot = !currentFormationSlot.HasValue;
        if (currentFormationSlot.HasValue && formationManager != null && formationManager.IsSlotStale(currentFormationSlot.Value, this))
        {
            formationManager.ReleaseFormationSlot(this);
            currentFormationSlot = null;
            needsNewSlot = true;
        }
        if (needsNewSlot && formationManager != null)
            currentFormationSlot = formationManager.RequestFormationSlot(this);

        if (currentFormationSlot.HasValue)
        {
            if (navMeshAgent.destination != currentFormationSlot.Value)
                navMeshAgent.SetDestination(currentFormationSlot.Value);
            if (Vector3.Distance(transform.position, currentFormationSlot.Value) < Tier1_SlotReachedThreshold)
            { /* 공격 로직 */ }
        }
        else
        {
            navMeshAgent.stoppingDistance = Tier2_ApproachStoppingDistance;
            if (navMeshAgent.destination != playerTransform.position)
                navMeshAgent.SetDestination(playerTransform.position);
        }
    }

    void UpdateTier2Behavior()
    {
        if (!navMeshAgent.enabled) navMeshAgent.enabled = true;
        if (navMeshAgent.isStopped) navMeshAgent.isStopped = false;
        navMeshAgent.stoppingDistance = Tier2_ApproachStoppingDistance;
        if (navMeshAgent.destination != playerTransform.position)
            navMeshAgent.SetDestination(playerTransform.position);
    }

    void UpdateTier3Behavior()
    {
        if (navMeshAgent.enabled && navMeshAgent.isOnNavMesh && !navMeshAgent.isStopped)
            navMeshAgent.isStopped = true;
    }

    public void NotifyNewFormationSlotPosition(Vector3 newWorldPosition)
    {
        currentFormationSlot = newWorldPosition;
        if (CurrentTier == AITier.Tier1_ActiveFormation && !isRetreating && navMeshAgent.enabled && navMeshAgent.isOnNavMesh)
        {
            if (navMeshAgent.destination != currentFormationSlot.Value)
                navMeshAgent.SetDestination(currentFormationSlot.Value);
        }
    }
}