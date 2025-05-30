using UnityEngine;
using UnityEngine.AI;

public class NormalMonsterAI : MonsterAI
{
    [Header("Normal Monster Tier 1 Config (Formation)")]
    public float Tier1Speed = 3.5f; 
    public float Tier1Acceleration = 8f;
    public float SlotReachedThreshold = 1.5f; // 슬롯 도달 판단 거리
    public float NoSlotApproachDistanceMultiplier = 0.9f; // 슬롯 없을 시 공격 거리 배율 (Enemy.EnemyData.AttackDistance * 이 값)

    private Vector3? currentFormationSlot = null;

    protected override float GetTier1Speed() => Tier1Speed;
    protected override float GetTier1Acceleration() => Tier1Acceleration;

    protected override void ConfigureNavMeshAgentForTier1()
    {
        // UpdateTier1Behavior에서 상황에 따라 stoppingDistance가 설정됩니다.
        // 기본값으로 슬롯 도달 임계값을 설정해둘 수 있습니다.
        navMeshAgent.stoppingDistance = SlotReachedThreshold * 0.8f;
    }

    protected override void OnExitTier1()
    {
        ReleaseFormationSlot();
    }

    protected override void HandleOnDisable()
    {
        // base.HandleOnDisable(); // 필요 시 기본 클래스 로직 호출
        ReleaseFormationSlot();
    }

    private void ReleaseFormationSlot()
    {
        if (currentFormationSlot.HasValue && formationManager != null)
        {
            formationManager.ReleaseFormationSlot(this);
            currentFormationSlot = null;
            // Debug.Log($"[{this.GetType().Name}-{this.GetInstanceID()}] Released formation slot.");
        }
    }

    protected override void UpdateTier1Behavior()
    {
        if (playerTransform == null) return;
        if (!navMeshAgent.enabled || !navMeshAgent.isOnNavMesh) return;

        // 포메이션 매니저가 없으면 플레이어 직접 추적
        if (formationManager == null)
        {
            // Debug.LogWarning($"[{this.GetType().Name}-{this.GetInstanceID()}] FormationManager is null. Normal monster will approach player directly.");
            if (Enemy != null && Enemy.EnemyData != null)
            {
                float desiredAttackDistance = Mathf.Max(0.5f, Enemy.EnemyData.AttackDistance * NoSlotApproachDistanceMultiplier);
                navMeshAgent.stoppingDistance = desiredAttackDistance;
            }
            else
            {
                navMeshAgent.stoppingDistance = Tier2_ApproachStoppingDistance; // Fallback
            }
            if (navMeshAgent.destination != playerTransform.position)
            {
                navMeshAgent.SetDestination(playerTransform.position);
            }
            return;
        }
        
        // --- 포메이션 로직 ---
        bool needsNewSlot = !currentFormationSlot.HasValue;
        if (currentFormationSlot.HasValue)
        {
            if (formationManager.IsSlotStale(currentFormationSlot.Value, this)) 
            {
                ReleaseFormationSlot();
                needsNewSlot = true;
            }
        }

        if (needsNewSlot)
        {
            currentFormationSlot = formationManager.RequestFormationSlot(this); 
            // if (currentFormationSlot.HasValue) { Debug.Log($"[{this.GetType().Name}-{this.GetInstanceID()}] Acquired new formation slot: {currentFormationSlot.Value}"); }
            // else { Debug.Log($"[{this.GetType().Name}-{this.GetInstanceID()}] Failed to acquire formation slot.");}
        }

        if (currentFormationSlot.HasValue) 
        {
            navMeshAgent.stoppingDistance = SlotReachedThreshold * 0.8f; 
            if (navMeshAgent.destination != currentFormationSlot.Value)
            {
                navMeshAgent.SetDestination(currentFormationSlot.Value);
            }
        }
        else 
        {
            // Debug.Log($"[{this.GetType().Name}-{this.GetInstanceID()}] No formation slot. Approaching player.");
            if (Enemy != null && Enemy.EnemyData != null)
            {
                 float desiredAttackDistance = Mathf.Max(0.5f, Enemy.EnemyData.AttackDistance * NoSlotApproachDistanceMultiplier);
                 navMeshAgent.stoppingDistance = desiredAttackDistance; 
            }
            else
            {
                navMeshAgent.stoppingDistance = Tier2_ApproachStoppingDistance; // Fallback
            }

            if (navMeshAgent.destination != playerTransform.position)
            {
                navMeshAgent.SetDestination(playerTransform.position);
            }
        }
        
    }

    
    public override void NotifyNewFormationSlotPosition(Vector3 newWorldPosition)
    {
        // base.NotifyNewFormationSlotPosition(newWorldPosition); // 기본 클래스 호출 (선택 사항)

        if (newWorldPosition == Vector3.positiveInfinity) // 유효하지 않은 슬롯 위치 (슬롯 상실 의미)
        {
            // Debug.Log($"[{this.GetType().Name}-{this.GetInstanceID()}] Lost formation slot.");
            currentFormationSlot = null; 
            // 슬롯을 잃었을 때의 추가적인 행동 (예: 플레이어 직접 추적 모드로 전환 결정 등)
        }
        else
        {
            // Debug.Log($"[{this.GetType().Name}-{this.GetInstanceID()}] Notified of new slot position: {newWorldPosition}");
            currentFormationSlot = newWorldPosition;
            // Tier1 상태이고, NavMeshAgent가 활성화되어 있으며, 현재 슬롯이 있을 때만 목적지 업데이트
            if (CurrentTier == AITier.Tier1_ActiveFormation &&
                navMeshAgent != null && navMeshAgent.enabled && 
                navMeshAgent.isOnNavMesh &&
                currentFormationSlot.HasValue) // currentFormationSlot.HasValue는 여기서 항상 true일 것임
            {
                if (navMeshAgent.destination != currentFormationSlot.Value)
                {
                    navMeshAgent.SetDestination(currentFormationSlot.Value);
                }
            }
        }
    }
    
    
    
}