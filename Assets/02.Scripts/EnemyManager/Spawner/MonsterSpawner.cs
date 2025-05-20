// MonsterSpawner.cs

using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random; // NavMesh 관련 사용

[Serializable]
public class MonsterSpawner : MonoBehaviour
{
    [Header("Dependencies")]
    public ObjectPool monsterPool;     // Inspector에서 할당
    public AIManager aiManager;         // Inspector에서 할당

    [Header("Spawning Settings")]
    public int numberOfMonstersToSpawn = 50;
    public float spawnRadius = 30f;     // 스포너 중심으로부터의 스폰 반경
    public Transform spawnPoint;        // 스폰 중심점 (지정하지 않으면 이 오브젝트의 위치 사용)
    public bool InitSpawn = true;
    void Start()
    {
        if (monsterPool == null || aiManager == null)
        {
            Debug.LogError("MonsterPool or AIManager not assigned in MonsterSpawner!");
            this.enabled = false; // 스크립트 비활성화
            return;
        }

        if (spawnPoint == null)
        {
            spawnPoint = transform; // 지정 안되면 자신의 위치를 사용
        }

        //SpawnInitialMonsters();
        
    }

    public void StartSpawning()
    {
        SpawnInitialMonsters();
    }

    void SpawnInitialMonsters()
    {
        for (int i = 0; i < numberOfMonstersToSpawn; i++)
        {
            // 원형 영역 내 랜덤 위치 선정 (Y축은 spawnPoint와 동일하게)
            Vector2 randomCirclePos = Random.insideUnitCircle * spawnRadius;
            Vector3 randomPos = spawnPoint.position + new Vector3(randomCirclePos.x, 0, randomCirclePos.y);

            NavMeshHit hit;
            // NavMesh 위의 유효한 위치인지 샘플링
            if (NavMesh.SamplePosition(randomPos, out hit, spawnRadius, NavMesh.AllAreas))
            {
                SpawnMonsterAt(hit.position);
            }
            else
            {
                // 유효 위치 못 찾으면 스폰 포인트에라도 스폰 (또는 다른 처리)
                Debug.LogWarning($"Could not find NavMesh position near {randomPos}. Spawning at fallback.");
                SpawnMonsterAt(spawnPoint.position);
            }
        }
    }

    public MonsterAI SpawnMonsterAt(Vector3 position)
    {
        GameObject monsterGO = monsterPool.GetObject();
        if (monsterGO == null)
        {
            Debug.LogError("Failed to get monster from pool. Pool might be exhausted and not allowed to grow.");
            return null;
        }

        monsterGO.transform.position = position;
        monsterGO.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0); // 랜덤 Y축 회전
        monsterGO.SetActive(true);

        MonsterAI monsterAI = monsterGO.GetComponent<MonsterAI>();
        if (monsterAI != null)
        {
            // AIManager에 몬스터를 등록하고 초기화 (플레이어 정보 등 전달)
            aiManager.InitializeSpawnedMonster(monsterAI);
        }
        else
        {
            Debug.LogError("Spawned object is missing MonsterAI component!");
            // 문제가 있는 오브젝트는 풀에 반환하거나 파괴
            monsterPool.ReturnObject(monsterGO); // 또는 Destroy(monsterGO);
            return null;
        }

        //행동 그래프 플레이어 설정
        //monsterAI.BehaviorGraphAgent.BlackboardReference.SetVariableValue("Target",aiManager.playerTransform.gameObject);
        
        return monsterAI;
    }
}