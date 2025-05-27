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
    [Header("Runtime Status")]
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

    private Renderer monsterRenderer;

    [Header("Performance")]
    public float logicUpdateInterval = 0.2f;
    public float nextIndividualLogicUpdateTime = 0f;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = true;

        monsterRenderer = GetComponentInChildren<Renderer>();
        if (monsterRenderer == null)
        {
            Debug.LogWarning("MonsterAI: Renderer component not found on " + gameObject.name, this.gameObject);
        }
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
        navMeshAgent.enabled = false;
        navMeshAgent.enabled = true;

        playerTransform = player;
        formationManager = fm;
        aiManager = am;
        nextIndividualLogicUpdateTime = Time.time + Random.Range(0, logicUpdateInterval);

        Enemy.Target = player.gameObject;

        if (Enemy != null)
        {
            Enemy.Pool = pool;
        }
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
                navMeshAgent.speed = Tier1_Speed;
                navMeshAgent.acceleration = 8f;
                navMeshAgent.autoRepath = true;
                navMeshAgent.stoppingDistance = Tier1_SlotReachedThreshold * 0.8f;
                navMeshAgent.isStopped = false;
                logicUpdateInterval = 0.1f;
                break;

            case AITier.Tier2_Approaching:
                navMeshAgent.speed = Tier2_Speed;
                navMeshAgent.acceleration = 5f;
                navMeshAgent.stoppingDistance = Tier2_ApproachStoppingDistance;
                navMeshAgent.autoRepath = true;
                navMeshAgent.isStopped = false; // ✅ 에이전트 다시 활성화
                logicUpdateInterval = 0.3f;
                break;

            case AITier.Tier3_Background:
                if (navMeshAgent.enabled && navMeshAgent.isOnNavMesh)
                    navMeshAgent.isStopped = true;
                logicUpdateInterval = 1.0f;
                break;
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

        // 추가 안전장치: Tier2인데 isStopped면 다시 활성화
        if (CurrentTier == AITier.Tier2_Approaching &&
            navMeshAgent.enabled && navMeshAgent.isOnNavMesh &&
            navMeshAgent.isStopped)
        {
            navMeshAgent.isStopped = false;
        }
    }

    void UpdateTier1Behavior()
    {
        if (playerTransform == null) return;

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
        }
        else
        {
            if (navMeshAgent.enabled && navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.stoppingDistance = Tier2_ApproachStoppingDistance;
                if (navMeshAgent.destination != playerTransform.position)
                    navMeshAgent.SetDestination(playerTransform.position);
            }
        }
    }

    void UpdateTier2Behavior()
    {
        if (playerTransform == null) return;

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
