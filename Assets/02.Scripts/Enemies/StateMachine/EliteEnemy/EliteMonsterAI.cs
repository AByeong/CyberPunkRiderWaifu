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

        if (navMeshAgent.destination != playerTransform.position)
        {
            navMeshAgent.SetDestination(playerTransform.position);
        }
    }
}