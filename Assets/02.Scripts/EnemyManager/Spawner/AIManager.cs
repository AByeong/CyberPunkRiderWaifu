using UnityEngine;
using System.Collections.Generic;
// using System.Linq; // 현재 코드에서는 Linq를 직접 사용하지 않으므로 주석 처리 가능

public class AIManager : MonoBehaviour
{
    [Header("Player & Managers")]
    public GameObject player; // 플레이어 게임 오브젝트 참조
    public Transform playerTransform; // 플레이어 트랜스폼 (성능을 위해 캐싱)
    public FormationManager formationManager; // 포메이션 매니저 참조

    [Header("Tier Settings")]
    public float tier1MaxDistance = 20f; // Tier1으로 간주되는 플레이어와의 최대 거리
    public float tier2MaxDistance = 50f; // Tier2로 간주되는 플레이어와의 최대 거리
    // Tier3은 tier2MaxDistance보다 먼 경우로 자동 결정됨

    [Header("Update Intervals")]
    public float tierCheckInterval = 0.5f; // 모든 몬스터의 Tier를 재확인하는 주기
    public int monstersPerFrameLogicUpdate = 10; // 매 프레임 로직을 업데이트할 최대 몬스터 수

    private List<MonsterAI> allMonsters = new List<MonsterAI>(); // 관리 중인 모든 몬스터 리스트
    private float nextTierCheckTime = 0f; // 다음 Tier 전체 검사 시간
    private int currentMonsterUpdateIndex = 0; // 순차적 로직 업데이트를 위한 현재 몬스터 인덱스

    private float tier1MaxDistanceSqr; // tier1MaxDistance의 제곱값 (거리 계산 최적화)
    private float tier2MaxDistanceSqr; // tier2MaxDistance의 제곱값

    void Awake()
    {
        if (player == null)
        {
            Debug.LogError("Player GameObject not assigned in AIManager! Attempting to find by tag 'Player'.");
            player = GameObject.FindGameObjectWithTag("Player"); // 플레이어 태그로 찾기 시도
        }

        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player Transform could not be initialized in AIManager!");
        }

        if (formationManager == null)
        {
            Debug.LogError("Formation Manager not assigned in AIManager! Attempting to find in scene.");
            formationManager = FindObjectOfType<FormationManager>(); // 씬에서 찾기 시도
            if (formationManager == null) Debug.LogError("Formation Manager could not be found in scene!");
        }

        // 거리 비교 최적화를 위해 제곱값 미리 계산
        tier1MaxDistanceSqr = tier1MaxDistance * tier1MaxDistance;
        tier2MaxDistanceSqr = tier2MaxDistance * tier2MaxDistance;
    }

    /// <summary>
    /// AI 매니저에 몬스터를 등록합니다.
    /// </summary>
    public void RegisterMonster(MonsterAI monster)
    {
        if (monster != null && !allMonsters.Contains(monster))
        {
            allMonsters.Add(monster);
        }
    }

    /// <summary>
    /// AI 매니저에서 몬스터를 제거합니다.
    /// </summary>
    public void UnregisterMonster(MonsterAI monster)
    {
        if (monster != null && allMonsters.Contains(monster))
        {
            allMonsters.Remove(monster);
        }
    }

    /// <summary>
    /// 스폰된 몬스터를 초기화하고 AI 매니저에 등록합니다.
    /// </summary>
    public void InitializeSpawnedMonster(MonsterAI monster, ObjectPool pool) // ObjectPool 타입은 프로젝트에 맞게 사용
    {
        if (monster == null)
        {
            Debug.LogError($"{gameObject.name} :: 몬스터 없지용");
            return;
        }
        

        if (playerTransform != null)
        {
            playerTransform =GameManager.Instance.player.transform;
        }
        
        // MonsterAI의 Initialize 호출 (여기서 nextIndividualLogicUpdateTime 등이 초기화됨)
        monster.Initialize(playerTransform, formationManager, this, pool);
        
        if (!allMonsters.Contains(monster)) // 중복 등록 방지
        {
            allMonsters.Add(monster);
        }
        DetermineAndSetMonsterTier(monster); // 초기 Tier 설정
    }

    void Update()
    {
        if (playerTransform == null || allMonsters.Count == 0) return;

        // 주기적으로 모든 몬스터의 Tier 업데이트
        if (Time.time > nextTierCheckTime)
        {
            UpdateAllMonsterTiers();
            nextTierCheckTime = Time.time + tierCheckInterval;
        }
        // 매 프레임 일부 몬스터의 로직을 순차적으로 업데이트
        UpdateMonsterLogicsStaggered();
    }

    /// <summary>
    /// 모든 활성 몬스터의 AI Tier를 업데이트합니다.
    /// </summary>
    void UpdateAllMonsterTiers()
    {
        // 반복 중 컬렉션 변경에 안전하도록 리스트의 복사본(스냅샷)을 사용
        List<MonsterAI> currentMonstersSnapshot = new List<MonsterAI>(allMonsters);
        foreach (MonsterAI monster in currentMonstersSnapshot)
        {
            // 몬스터가 null이거나 비활성화 상태면 건너뜀
            if (monster == null || !monster.gameObject.activeInHierarchy) continue;
            DetermineAndSetMonsterTier(monster);
        }
    }

    /// <summary>
    /// 특정 몬스터의 AI Tier를 플레이어와의 거리에 따라 결정하고 설정합니다.
    /// </summary>
    void DetermineAndSetMonsterTier(MonsterAI monster)
    {
        if (monster == null || playerTransform == null) return;

        float sqrDistanceToPlayer = (monster.transform.position - playerTransform.position).sqrMagnitude;
        AITier newTier = AITier.Tier3_Background; // 기본값은 Tier3

        if (sqrDistanceToPlayer < tier1MaxDistanceSqr)
        {
            newTier = AITier.Tier1_ActiveFormation;
        }
        else if (sqrDistanceToPlayer < tier2MaxDistanceSqr)
        {
            newTier = AITier.Tier2_Approaching;
        }
        // 그 외는 Tier3_Background 유지

        // 몬스터에게 새로운 Tier 설정 (forceUpdate=true로 호출하여 동일 Tier라도 내부 로직 재실행 가능)
        monster.SetAITier(newTier, true);
    }

    /// <summary>
    /// 몬스터들의 AI 로직 업데이트를 프레임별로 분산시켜 처리합니다.
    /// </summary>
    void UpdateMonsterLogicsStaggered()
    {
        if (allMonsters.Count == 0) return;

        int processedThisFrame = 0;
        // 한 프레임에 모든 몬스터를 다 순회 시도하되, 실제 로직 실행은 monstersPerFrameLogicUpdate 수만큼 제한
        for (int i = 0; i < allMonsters.Count && processedThisFrame < monstersPerFrameLogicUpdate; ++i)
        {
            if (allMonsters.Count == 0) break; // 업데이트 중 리스트가 비워질 경우 대비

            currentMonsterUpdateIndex %= allMonsters.Count; // 인덱스가 리스트 범위를 벗어나지 않도록 보정

            MonsterAI monsterToUpdate = allMonsters[currentMonsterUpdateIndex];
            if (monsterToUpdate != null && monsterToUpdate.gameObject.activeInHierarchy)
            {
                // 각 몬스터의 개별 업데이트 주기에 따라 로직 실행
                if (Time.time >= monsterToUpdate.nextIndividualLogicUpdateTime)
                {
                    monsterToUpdate.ManagedUpdateLogic();
                    // 다음 개별 로직 업데이트 시간 설정
                    monsterToUpdate.nextIndividualLogicUpdateTime = Time.time + monsterToUpdate.logicUpdateInterval;
                    processedThisFrame++;
                }
            }
            currentMonsterUpdateIndex++; // 다음 프레임에 검사할 몬스터 인덱스로 이동
        }
    }
}


// AITypes.cs (또는 AI 관련 공용 스크립트 파일)
public enum AITier
{
    Tier1_ActiveFormation, // Elite의 경우 포메이션 사용, Normal의 경우 적극적 교전
    Tier2_Approaching,
    Tier3_Background
}