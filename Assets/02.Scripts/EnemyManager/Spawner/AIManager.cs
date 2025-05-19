using UnityEngine;
using System.Collections.Generic;

public class AIManager : MonoBehaviour
{
    [Header("Player & Managers")]
    public Transform playerTransform;
    public FormationManager formationManager;

    [Header("Tier Settings")]
    public float tier1MaxDistance = 20f;
    public float tier2MaxDistance = 50f;
    
    [Header("Update Intervals")]
    public float tierCheckInterval = 0.5f;
    public int monstersPerFrameLogicUpdate = 10; // 한 번의 Update 호출에서 로직을 실행할 몬스터 수

    private List<MonsterAI> allMonsters = new List<MonsterAI>();
    private float nextTierCheckTime = 0f;
    private int currentMonsterUpdateIndex = 0;
    
    private float tier1MaxDistanceSqr;
    private float tier2MaxDistanceSqr;

    void Awake()
    {
        if (playerTransform == null) Debug.LogError("Player Transform not assigned in AIManager!");
        if (formationManager == null) Debug.LogError("Formation Manager not assigned in AIManager!");
        
        tier1MaxDistanceSqr = tier1MaxDistance * tier1MaxDistance;
        tier2MaxDistanceSqr = tier2MaxDistance * tier2MaxDistance;
    }
    
    public void RegisterMonster(MonsterAI monster)
    {
        if (!allMonsters.Contains(monster)) allMonsters.Add(monster);
    }

    public void UnregisterMonster(MonsterAI monster)
    {
        if (allMonsters.Contains(monster)) allMonsters.Remove(monster);
    }
    
    public void InitializeSpawnedMonster(MonsterAI monster)
    {
        if (monster == null) return;
        monster.Initialize(playerTransform, formationManager, this); // MonsterAI의 nextIndividualLogicUpdateTime도 여기서 초기화됨
        if (!allMonsters.Contains(monster)) allMonsters.Add(monster);
        DetermineAndSetMonsterTier(monster);
    }

    void Update()
    {
        if (playerTransform == null || allMonsters.Count == 0) return;

        if (Time.time > nextTierCheckTime)
        {
            UpdateAllMonsterTiers();
            nextTierCheckTime = Time.time + tierCheckInterval;
        }
        UpdateMonsterLogicsStaggered();
    }

    void UpdateAllMonsterTiers()
    {
        List<MonsterAI> currentMonstersSnapshot = new List<MonsterAI>(allMonsters); // 반복 중 변경에 안전하도록 스냅샷 사용
        foreach (MonsterAI monster in currentMonstersSnapshot)
        {
            if (monster == null || !monster.gameObject.activeInHierarchy) continue;
            DetermineAndSetMonsterTier(monster);
        }
    }
    
    void DetermineAndSetMonsterTier(MonsterAI monster)
    {
        float sqrDistanceToPlayer = (monster.transform.position - playerTransform.position).sqrMagnitude;
        AITier newTier = AITier.Tier3_Background;

        if (sqrDistanceToPlayer < tier1MaxDistanceSqr) newTier = AITier.Tier1_ActiveFormation;
        else if (sqrDistanceToPlayer < tier2MaxDistanceSqr) newTier = AITier.Tier2_Approaching;
        
        monster.SetAITier(newTier, true); // forceUpdate를 true로 하여 내부 설정(속도, logicUpdateInterval 등)이 반영되도록
    }

    void UpdateMonsterLogicsStaggered()
    {
        if (allMonsters.Count == 0) return;

        int processedCount = 0; // 이번 프레임에 실제로 로직 업데이트한 몬스터 수
        for (int i = 0; i < allMonsters.Count && processedCount < monstersPerFrameLogicUpdate; i++) // 전체 리스트를 순회하되, 최대 N개만 처리
        {
            currentMonsterUpdateIndex %= allMonsters.Count; // 인덱스 순환 보장

            MonsterAI monsterToUpdate = allMonsters[currentMonsterUpdateIndex];
            if (monsterToUpdate != null && monsterToUpdate.gameObject.activeInHierarchy)
            {
                // 해당 몬스터의 개별 업데이트 시간이 되었는지 확인
                if (Time.time >= monsterToUpdate.nextIndividualLogicUpdateTime)
                {
                    monsterToUpdate.ManagedUpdateLogic();
                    monsterToUpdate.nextIndividualLogicUpdateTime = Time.time + monsterToUpdate.logicUpdateInterval;
                    processedCount++;
                }
            }
            currentMonsterUpdateIndex++;
        }
    }
}