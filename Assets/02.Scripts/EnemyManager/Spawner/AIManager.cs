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
    public int monstersPerFrameLogicUpdate = 10;

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
        if (monster != null && !allMonsters.Contains(monster)) allMonsters.Add(monster);
    }

    public void UnregisterMonster(MonsterAI monster)
    {
        if (monster != null && allMonsters.Contains(monster)) allMonsters.Remove(monster);
    }
    
    public void InitializeSpawnedMonster(MonsterAI monster)
    {
        if (monster == null) return;
        // MonsterAI의 Initialize 호출 (여기서 nextIndividualLogicUpdateTime 초기화됨)
        monster.Initialize(playerTransform, formationManager, this); 
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
        // 반복 중 컬렉션 변경에 안전하도록 스냅샷 사용 (또는 역순회)
        List<MonsterAI> currentMonstersSnapshot = new List<MonsterAI>(allMonsters); 
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
        
        monster.SetAITier(newTier, true); 
    }

    void UpdateMonsterLogicsStaggered()
    {
        if (allMonsters.Count == 0) return;

        int processedThisFrame = 0;
        // 한 프레임에 모든 몬스터를 다 순회하되, 실제 로직은 monstersPerFrameLogicUpdate 만큼만 실행
        for (int i = 0; i < allMonsters.Count && processedThisFrame < monstersPerFrameLogicUpdate; ++i)
        {
            currentMonsterUpdateIndex %= allMonsters.Count; // Ensure index is within bounds

            MonsterAI monsterToUpdate = allMonsters[currentMonsterUpdateIndex];
            if (monsterToUpdate != null && monsterToUpdate.gameObject.activeInHierarchy)
            {
                if (Time.time >= monsterToUpdate.nextIndividualLogicUpdateTime)
                {
                    monsterToUpdate.ManagedUpdateLogic();
                    monsterToUpdate.nextIndividualLogicUpdateTime = Time.time + monsterToUpdate.logicUpdateInterval;
                    processedThisFrame++;
                }
            }
            currentMonsterUpdateIndex++;
        }
    }
}