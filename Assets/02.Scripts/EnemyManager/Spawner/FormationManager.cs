using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FormationManager : MonoBehaviour
{
    [Header("Formation Settings")]
    public Transform playerTransform;
    public int maxFormationSlots = 15;
    public float formationRadius = 7f;
    public float slotUpdateInterval = 0.5f;

    private List<Vector3> calculatedPhysicalSlots = new List<Vector3>();
    private Dictionary<MonsterAI, Vector3> assignedSlotsData = new Dictionary<MonsterAI, Vector3>();
    private Queue<MonsterAI> monstersWaitingForSlot = new Queue<MonsterAI>();
    private float nextSlotUpdateTime = 0f;

    private HashSet<Vector3> occupiedSlotsCache = new HashSet<Vector3>();

    void Update()
    {
        if (playerTransform == null) return;

        if (Time.time > nextSlotUpdateTime)
        {
            UpdateAndReassignFormationSlots();
            AssignWaitingMonstersToAvailableSlots();
            nextSlotUpdateTime = Time.time + slotUpdateInterval;
        }
    }

    public bool IsSlotStale(Vector3 slotPositionToCheck, MonsterAI monster)
    {
        if (playerTransform == null) return true;

        float distanceFromPlayerToSlot = Vector3.Distance(playerTransform.position, slotPositionToCheck);
        // 포메이션 반경을 너무 많이 벗어나면 Stale 처리
        if (distanceFromPlayerToSlot > formationRadius * 1.5f) return true;
        return false;
    }

    void UpdateAndReassignFormationSlots()
    {
        if (playerTransform == null) return;

        List<Vector3> newPhysicalSlots = new List<Vector3>();
        for (int i = 0; i < maxFormationSlots; i++)
        {
            float angle = i * (360f / maxFormationSlots) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * formationRadius;
            Vector3 potentialSlotPos = playerTransform.position + offset;
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(potentialSlotPos, out hit, 2.0f, UnityEngine.AI.NavMesh.AllAreas))
            {
                newPhysicalSlots.Add(hit.position);
            }
        }
        calculatedPhysicalSlots = newPhysicalSlots;

        // 유효한 물리적 슬롯이 없으면 모든 할당을 해제하고 몬스터에게 알림
        if (calculatedPhysicalSlots.Count == 0)
        {
            foreach (MonsterAI monsterInSlot in assignedSlotsData.Keys.ToList())
            {
                if (monsterInSlot != null)
                {
                    // 몬스터에게 슬롯이 유효하지 않음을 알림 (예: 무한대 위치 전달)
                    // NormalMonsterAI가 이 알림을 받고 currentFormationSlot을 null로 처리할 수 있도록 구현 필요
                    monsterInSlot.NotifyNewFormationSlotPosition(Vector3.positiveInfinity); 
                }
            }
            assignedSlotsData.Clear();
            // 대기열도 비워야 할 수 있음 (상황에 따라)
            // while(monstersWaitingForSlot.Count > 0) { monstersWaitingForSlot.Dequeue().NotifyNewFormationSlotPosition(Vector3.positiveInfinity); }
            return;
        }

        Dictionary<MonsterAI, Vector3> nextAssignedSlots = new Dictionary<MonsterAI, Vector3>();
        List<MonsterAI> monstersToEnqueueLater = new List<MonsterAI>();
        List<Vector3> availableNewPhysicalSlots = new List<Vector3>(calculatedPhysicalSlots);

        // 현재 슬롯을 가진 몬스터들에게 새 위치 할당 또는 대기열로 이동
        foreach (KeyValuePair<MonsterAI, Vector3> currentAssignment in assignedSlotsData.ToList())
        {
            MonsterAI monster = currentAssignment.Key;
            if (monster == null || !monster.gameObject.activeInHierarchy || monster.CurrentTier != AITier.Tier1_ActiveFormation)
            {
                // 유효하지 않은 몬스터는 다음 할당에서 제외 (자동으로 슬롯 해제 효과)
                continue;
            }

            Vector3 bestNewSlotForThisMonster = Vector3.zero;
            float minDistanceToMonster = float.MaxValue;
            bool foundSuitableNewSlot = false;
            int bestSlotIndexInAvailableList = -1;

            for (int i = 0; i < availableNewPhysicalSlots.Count; ++i)
            {
                float dist = Vector3.Distance(monster.transform.position, availableNewPhysicalSlots[i]);
                if (dist < minDistanceToMonster)
                {
                    minDistanceToMonster = dist;
                    bestNewSlotForThisMonster = availableNewPhysicalSlots[i];
                    bestSlotIndexInAvailableList = i;
                    foundSuitableNewSlot = true;
                }
            }

            if (foundSuitableNewSlot)
            {
                nextAssignedSlots[monster] = bestNewSlotForThisMonster;
                monster.NotifyNewFormationSlotPosition(bestNewSlotForThisMonster); // MonsterAI의 가상 메서드 호출
                availableNewPhysicalSlots.RemoveAt(bestSlotIndexInAvailableList);
            }
            else
            {
                // 적절한 새 슬롯을 찾지 못한 몬스터는 대기열로 보냄
                monstersToEnqueueLater.Add(monster);
                // 슬롯을 잃었음을 알림
                monster.NotifyNewFormationSlotPosition(Vector3.positiveInfinity);
            }
        }
        assignedSlotsData = nextAssignedSlots; // 새 할당 정보로 갱신

        // 슬롯을 재할당받지 못한 몬스터들을 대기열에 추가
        foreach (MonsterAI monster in monstersToEnqueueLater)
        {
            if (monster != null && monster.gameObject.activeInHierarchy && 
                monster.CurrentTier == AITier.Tier1_ActiveFormation && !monstersWaitingForSlot.Contains(monster) &&
                !assignedSlotsData.ContainsKey(monster)) // 이미 새 슬롯을 받지 않았는지 확인
            {
                monstersWaitingForSlot.Enqueue(monster);
            }
        }
    }

    void AssignWaitingMonstersToAvailableSlots()
    {
        if (monstersWaitingForSlot.Count == 0 || calculatedPhysicalSlots.Count == 0) return;

        occupiedSlotsCache.Clear();
        foreach (Vector3 val in assignedSlotsData.Values) occupiedSlotsCache.Add(val);

        List<Vector3> freePhysicalSlots = new List<Vector3>();
        foreach (Vector3 slot in calculatedPhysicalSlots)
        {
            if (!occupiedSlotsCache.Contains(slot)) freePhysicalSlots.Add(slot);
        }

        int assignedThisCycle = 0; // 한 번의 호출에서 과도한 할당을 방지 (선택적)
        while (monstersWaitingForSlot.Count > 0 && freePhysicalSlots.Count > 0 && assignedThisCycle < maxFormationSlots)
        {
            MonsterAI monster = monstersWaitingForSlot.Dequeue();
            if (monster == null || !monster.gameObject.activeInHierarchy || monster.CurrentTier != AITier.Tier1_ActiveFormation)
            {
                // assignedSlotsData.Remove(monster); // 이미 assignedSlotsData에는 없을 가능성이 높음 (재할당 실패 후 대기열로 이동)
                continue;
            }
            // 이미 다른 방식으로 슬롯을 할당받았다면 건너뛰기 (매우 드문 경우)
            if (assignedSlotsData.ContainsKey(monster)) continue;

            Vector3 bestSlot = Vector3.zero;
            float minDistance = float.MaxValue;
            bool foundSlot = false;
            int bestSlotIndexInFreeList = -1;

            for (int i = 0; i < freePhysicalSlots.Count; ++i)
            {
                float distance = Vector3.Distance(monster.transform.position, freePhysicalSlots[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestSlot = freePhysicalSlots[i];
                    bestSlotIndexInFreeList = i;
                    foundSlot = true;
                }
            }

            if (foundSlot)
            {
                assignedSlotsData[monster] = bestSlot;
                monster.NotifyNewFormationSlotPosition(bestSlot);
                freePhysicalSlots.RemoveAt(bestSlotIndexInFreeList);
                assignedThisCycle++;
            }
            else
            {
                // 이번 사이클에 가능한 슬롯이 없으면 다시 대기열의 맨 뒤로 (Dequeue했으므로 다시 Enqueue)
                 if (!monstersWaitingForSlot.Contains(monster)) monstersWaitingForSlot.Enqueue(monster);
                break; // 더 이상 할당할 수 있는 슬롯이 없음
            }
        }
    }

    public Vector3? RequestFormationSlot(MonsterAI monster)
    {
        if (monster == null) return null;
        // 이미 슬롯이 할당된 몬스터의 요청이면 현재 슬롯 반환
        if (assignedSlotsData.ContainsKey(monster)) return assignedSlotsData[monster];

        // 사용 가능한 물리적 슬롯 중 현재 점유되지 않은 슬롯 찾기
        occupiedSlotsCache.Clear();
        foreach (Vector3 val in assignedSlotsData.Values) occupiedSlotsCache.Add(val);

        List<Vector3> freePhysicalSlots = new List<Vector3>();
        // calculatedPhysicalSlots가 최신 상태임을 가정 (Update 주기에 따라)
        foreach (Vector3 slot in calculatedPhysicalSlots)
        {
            if (!occupiedSlotsCache.Contains(slot)) freePhysicalSlots.Add(slot);
        }

        if (assignedSlotsData.Count < maxFormationSlots && freePhysicalSlots.Count > 0)
        {
            Vector3 bestSlot = Vector3.zero;
            float minDistance = float.MaxValue;
            bool foundSlot = false;

            // 가장 가까운 빈 슬롯 할당
            foreach (var slot in freePhysicalSlots)
            {
                float distance = Vector3.Distance(monster.transform.position, slot);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestSlot = slot;
                    foundSlot = true;
                }
            }
            if (foundSlot)
            {
                assignedSlotsData[monster] = bestSlot;
                monster.NotifyNewFormationSlotPosition(bestSlot);
                return bestSlot;
            }
        }

        // 즉시 할당 가능한 슬롯이 없으면 대기열에 추가
        if (!monstersWaitingForSlot.Contains(monster) && !assignedSlotsData.ContainsKey(monster))
        {
            monstersWaitingForSlot.Enqueue(monster);
        }
        return null; // 현재 할당 가능한 슬롯 없음
    }

    public void ReleaseFormationSlot(MonsterAI monster)
    {
        if (monster == null) return;
        if (assignedSlotsData.ContainsKey(monster))
        {
            assignedSlotsData.Remove(monster);
            // Debug.Log($"Released slot for monster: {monster.name}");
        }
        // 대기열에서도 제거할 수 있지만, 보통 Request 시점에 대기열에 들어가므로,
        // Release는 주로 몬스터가 비활성화되거나 Tier1을 벗어날 때 호출됨.
    }

    void OnDrawGizmosSelected()
    {
        if (playerTransform == null) return;

        // 전체 계산된 물리적 슬롯 위치를 파란색으로 표시
        Gizmos.color = Color.blue;
        if (calculatedPhysicalSlots != null)
        {
            foreach (var slot in calculatedPhysicalSlots)
            {
                Gizmos.DrawWireSphere(slot, 0.3f);
            }
        }

        // 현재 할당된 슬롯을 초록색으로 표시
        Gizmos.color = Color.green;
        if (assignedSlotsData != null)
        {
            foreach (var kvp in assignedSlotsData)
            {
                if (kvp.Key != null) // 몬스터가 아직 유효한 경우
                {
                    Gizmos.DrawSphere(kvp.Value, 0.35f);
                    Gizmos.DrawLine(kvp.Key.transform.position, kvp.Value); // 몬스터와 슬롯 연결선
                }
            }
        }

        // 플레이어 중심의 포메이션 반경 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(playerTransform.position, formationRadius);
    }
}