using Unity.Behavior; // BehaviorGraphAgent를 위해 필요
using UnityEngine;
using UnityEngine.AI;

public enum AITier
{
    Tier1_ActiveFormation,
    Tier2_Approaching,
    Tier3_Background
}

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAI : MonoBehaviour
{
    [Header("Runtime Status")] // 인스펙터에서 구분을 위한 헤더
    public AITier CurrentTier = AITier.Tier3_Background;
    
    public Enemy Enemy;
    private NavMeshAgent navMeshAgent;
    private Transform playerTransform;
    private FormationManager formationManager;
    private AIManager aiManager;

    [Header("Tier 1 Behavior (Active Formation & Retreat)")]
    public float Tier1_Speed = 3.5f;
    public float minPlayerDistance = 4.0f;
    public float desiredPlayerDistance = 6.0f;
    private const float Tier1_SlotReachedThreshold = 1.5f;
    private const float Tier1_LookAtPlayerSpeed = 5f; 
    private bool isRetreating = false;
    private Vector3? currentFormationSlot = null;

    [Header("Tier 2 Behavior (Approaching)")]
    public float Tier2_Speed = 7f;
    private const float Tier2_ApproachStoppingDistance = 8f;
    private const float Tier2_LookAtPlayerSpeed = 4f;

    [Header("Tier 3 Behavior")]
    private const float Tier3_LookAtPlayerSpeed = 3f;
    public float tier3LookAtPlayerMaxDistance = 10f;
    private float tier3LookAtPlayerMaxDistanceSqr;

    private Renderer monsterRenderer;

    [Header("Performance")]
    public float logicUpdateInterval = 0.2f;
    public float nextIndividualLogicUpdateTime = 0f;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false; 

        monsterRenderer = GetComponentInChildren<Renderer>(); 
        if (monsterRenderer == null)
        {
            Debug.LogWarning("MonsterAI: Renderer component not found on " + gameObject.name, this.gameObject);
        }
        
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

    public void Initialize(Transform player, FormationManager fm, AIManager am, ObjectPool pool)
    {
        playerTransform = player;
        formationManager = fm;
        aiManager = am;
        nextIndividualLogicUpdateTime = Time.time + Random.Range(0, logicUpdateInterval);

        Enemy.Target = player.gameObject;

        if (Enemy != null)
        {
            Enemy.Pool = pool;
        }
        // Debug.Log(gameObject.name + " 초기화 완");
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

        switch (CurrentTier)
        {
            case AITier.Tier1_ActiveFormation:
                navMeshAgent.speed = Tier1_Speed; // Tier1_Speed 사용
                navMeshAgent.acceleration = 8f; 
                navMeshAgent.autoRepath = true;
                logicUpdateInterval = 0.1f;
                break;
            case AITier.Tier2_Approaching:
                navMeshAgent.speed = Tier2_Speed; // Tier2_Speed 사용
                navMeshAgent.acceleration = 5f;
                navMeshAgent.stoppingDistance = Tier2_ApproachStoppingDistance; 
                navMeshAgent.autoRepath = true;
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
                default: currentLookSpeed = Tier1_LookAtPlayerSpeed; break; // 기본값 설정
            }
            ExecuteLookAtPlayerLogic(currentLookSpeed);
        }
    }

    void UpdateTier1Behavior()
    {
        if (playerTransform == null) return; // playerTransform null 체크 추가

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
                if (navMeshAgent.enabled && navMeshAgent.isOnNavMesh)
                {
                    navMeshAgent.SetDestination(hit.position);
                    navMeshAgent.stoppingDistance = 0.2f;
                }
            }
            else
            {
                Vector3 emergencyRetreatPos = transform.position + directionFromPlayer * 2.0f;
                if (NavMesh.SamplePosition(emergencyRetreatPos, out hit, 1.0f, NavMesh.AllAreas))
                {
                     if (navMeshAgent.enabled && navMeshAgent.isOnNavMesh)
                        navMeshAgent.SetDestination(hit.position);
                }
            }

            float desiredDistSqr = desiredPlayerDistance * desiredPlayerDistance;
            if (sqrDistToPlayer > desiredDistSqr * 0.9f) 
            {
                if (navMeshAgent.enabled && navMeshAgent.isOnNavMesh && !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + 0.5f)
                {
                    isRetreating = false;
                    navMeshAgent.ResetPath(); 
                }
            }
            return;
        }

        if (navMeshAgent.enabled && navMeshAgent.isOnNavMesh)
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
            if (navMeshAgent.enabled && navMeshAgent.isOnNavMesh && navMeshAgent.destination != currentFormationSlot.Value)
                navMeshAgent.SetDestination(currentFormationSlot.Value);
            
            // 공격 로직은 주석 처리된 상태로 유지
            // if (Vector3.Distance(transform.position, currentFormationSlot.Value) < Tier1_SlotReachedThreshold)
            // { /* 공격 로직 */ }
        }
        else // 슬롯이 없을 경우 플레이어에게 직접 이동
        {
            if (navMeshAgent.enabled && navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.stoppingDistance = Tier2_ApproachStoppingDistance; // Tier1이지만 슬롯 없으면 Tier2처럼 접근
                if (navMeshAgent.destination != playerTransform.position)
                    navMeshAgent.SetDestination(playerTransform.position);
            }
        }
    }

    void UpdateTier2Behavior()
    {
        if (playerTransform == null) return; // playerTransform null 체크 추가

        if (navMeshAgent.enabled && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.stoppingDistance = Tier2_ApproachStoppingDistance;
            if (navMeshAgent.destination != playerTransform.position)
                navMeshAgent.SetDestination(playerTransform.position);
        }
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