using UnityEngine;
using UnityEngine.AI;

public class EliteMonsterAI : MonsterAI
{
    [Header("Elite Monster Tier 1 Config (No Formation)")]
    public float Tier1Speed = 4.5f; // 엘리트 몬스터 Tier1 속도
    public float Tier1Acceleration = 9f;
    public float AttackDistanceMultiplier = 0.8f; // Enemy.EnemyData.AttackDistance * 이 값

    protected override float GetTier1Speed() => Tier1Speed;
    protected override float GetTier1Acceleration() => Tier1Acceleration;
    
    protected override void ConfigureNavMeshAgentForTier1()
    {
        // EliteMonsterAI는 UpdateTier1Behavior에서 stoppingDistance를 동적으로 설정합니다.
    }

    protected override void OnExitTier1()
    {
        // EliteMonster는 포메이션을 사용하지 않으므로 특별히 할 작업 없음
    }
    
    protected override void HandleOnDisable()
    {
        // EliteMonster는 포메이션을 사용하지 않으므로 특별히 할 작업 없음
    }

    protected override void UpdateTier1Behavior()
    {
        if (playerTransform == null || Enemy == null || Enemy.EnemyData == null) return;
        if (!navMeshAgent.enabled || !navMeshAgent.isOnNavMesh) return;

        float desiredAttackDistance = Mathf.Max(0.5f, Enemy.EnemyData.AttackDistance * AttackDistanceMultiplier); 
        navMeshAgent.stoppingDistance = desiredAttackDistance;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= desiredAttackDistance)
        {
            // 플레이어에게 충분히 가까이 다가갔으므로, 공격 로직을 시작하고 이동을 멈춥니다.
            // navMeshAgent.isStopped = true;
            // 여기에서 공격 애니메이션 재생, 데미지 계산 등의 공격 로직을 호출
            // 예: AttackPlayer();
            // 몬스터의 시야를 플레이어에게 고정
            Vector3 lookAtPlayer = playerTransform.position;
            lookAtPlayer.y = transform.position.y; // Y축은 고정하여 몬스터가 기울어지지 않게
            transform.LookAt(lookAtPlayer);
        }
        else
        {
            // 아직 공격 거리가 아니므로 플레이어에게 이동합니다.
            // navMeshAgent.isStopped = false;
            if (navMeshAgent.destination != playerTransform.position)
            {
                navMeshAgent.SetDestination(playerTransform.position);
            }
        }
    }
}