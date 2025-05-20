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

    // 최적화를 위해 미리 HashSet을 준비해둘 수 있지만, 매번 업데이트되므로 함수 내에서 생성
    private HashSet<Vector3> occupiedSlotsCache = new HashSet<Vector3>();

    void Update()
    {
        if (playerTransform == null) return;
        if (Time.time > nextSlotUpdateTime)
        {
            UpdateAndReassignFormationSlots();
            AssignWaitingMonstersToAvailableSlots(); // 대기 중인 몬스터들에게 슬롯 할당 시도
            nextSlotUpdateTime = Time.time + slotUpdateInterval;
        }
    }

    public bool IsSlotStale(Vector3 slotPositionToCheck, MonsterAI monster)
    {
        if (playerTransform == null) return true;
        float distanceFromPlayerToSlot = Vector3.Distance(playerTransform.position, slotPositionToCheck);
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

        if (calculatedPhysicalSlots.Count == 0)
        {
            foreach (MonsterAI monster in assignedSlotsData.Keys.ToList()) ReleaseFormationSlot(monster);
            assignedSlotsData.Clear();
            return;
        }

        Dictionary<MonsterAI, Vector3> nextAssignedSlots = new Dictionary<MonsterAI, Vector3>();
        List<MonsterAI> monstersToHandleLater = new List<MonsterAI>(); // 슬롯 못 받은 몬스터 임시 저장
        
        List<Vector3> availableNewPhysicalSlots = new List<Vector3>(calculatedPhysicalSlots);

        foreach (KeyValuePair<MonsterAI, Vector3> currentAssignment in assignedSlotsData.ToList()) // ToList로 안전하게 순회
        {
            MonsterAI monster = currentAssignment.Key;
            if (monster == null || !monster.gameObject.activeInHierarchy || monster.CurrentTier != AITier.Tier1_ActiveFormation)
            {
                // assignedSlotsData에서 직접 제거하지 않고, 나중에 nextAssignedSlots에 포함 안 시키는 방식으로 처리
                // ReleaseFormationSlot(monster); // 여기서 직접 호출하면 컬렉션 변경 문제 가능성
                continue; // 유효하지 않은 몬스터는 건너뜀 (새 할당 목록에 포함 안됨)
            }

            Vector3 bestNewSlotForThisMonster = Vector3.zero;
            float minDistanceToMonster = float.MaxValue;
            bool foundSuitableNewSlot = false;
            int bestSlotIndexInAvailableList = -1;

            for(int i = 0; i < availableNewPhysicalSlots.Count; ++i)
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
                monster.NotifyNewFormationSlotPosition(bestNewSlotForThisMonster);
                availableNewPhysicalSlots.RemoveAt(bestSlotIndexInAvailableList);
            }
            else
            {
                monstersToHandleLater.Add(monster); // 적절한 슬롯 못 찾으면 나중에 대기열로
            }
        }
        
        assignedSlotsData = nextAssignedSlots; // 새 할당 정보로 덮어쓰기

        // 슬롯을 재할당 받지 못한 기존 몬스터들을 대기열로 이동
        foreach(MonsterAI monster in monstersToHandleLater)
        {
             if (monster != null && monster.gameObject.activeInHierarchy && monster.CurrentTier == AITier.Tier1_ActiveFormation && !monstersWaitingForSlot.Contains(monster))
             {
                 monstersWaitingForSlot.Enqueue(monster);
             }
        }
    }

    void AssignWaitingMonstersToAvailableSlots()
    {
        if (monstersWaitingForSlot.Count == 0 || calculatedPhysicalSlots.Count == 0) return;

        occupiedSlotsCache.Clear();
        foreach(Vector3 val in assignedSlotsData.Values) occupiedSlotsCache.Add(val);

        List<Vector3> freePhysicalSlots = new List<Vector3>();
        foreach (Vector3 slot in calculatedPhysicalSlots)
        {
            if (!occupiedSlotsCache.Contains(slot)) freePhysicalSlots.Add(slot);
        }
        
        int MCount = 0;
        while (monstersWaitingForSlot.Count > 0 && freePhysicalSlots.Count > 0 && MCount < maxFormationSlots)
        {
            MCount++;
            MonsterAI monster = monstersWaitingForSlot.Dequeue();
            if (monster == null || !monster.gameObject.activeInHierarchy || monster.CurrentTier != AITier.Tier1_ActiveFormation)
            {
                assignedSlotsData.Remove(monster); continue;
            }
            if (assignedSlotsData.ContainsKey(monster)) continue;

            Vector3 bestSlot = Vector3.zero; float minDistance = float.MaxValue;
            bool foundSlot = false; int bestSlotIndexInFreeList = -1;

            for(int i=0; i < freePhysicalSlots.Count; ++i)
            {
                float distance = Vector3.Distance(monster.transform.position, freePhysicalSlots[i]);
                if (distance < minDistance)
                {
                    minDistance = distance; bestSlot = freePhysicalSlots[i];
                    bestSlotIndexInFreeList = i; foundSlot = true;
                }
            }

            if (foundSlot)
            {
                assignedSlotsData[monster] = bestSlot;
                monster.NotifyNewFormationSlotPosition(bestSlot);
                freePhysicalSlots.RemoveAt(bestSlotIndexInFreeList);
            }
            else
            {
                monstersWaitingForSlot.Enqueue(monster); break;
            }
        }
    }

    public Vector3? RequestFormationSlot(MonsterAI monster)
    {
        if (monster == null) return null;
        if (assignedSlotsData.ContainsKey(monster)) return assignedSlotsData[monster];

        // 이 시점의 calculatedPhysicalSlots가 최신이라고 가정 (UpdateAndReassignFormationSlots가 먼저 호출됨)
        occupiedSlotsCache.Clear();
        foreach(Vector3 val in assignedSlotsData.Values) occupiedSlotsCache.Add(val);
        
        List<Vector3> freePhysicalSlots = new List<Vector3>();
        foreach (Vector3 slot in calculatedPhysicalSlots) // calculatedPhysicalSlots 사용
        {
            if (!occupiedSlotsCache.Contains(slot)) freePhysicalSlots.Add(slot);
        }

        if (assignedSlotsData.Count < maxFormationSlots && freePhysicalSlots.Count > 0)
        {
            Vector3 bestSlot = Vector3.zero; float minDistance = float.MaxValue;
            bool foundSlot = false;

            foreach (var slot in freePhysicalSlots)
            {
                float distance = Vector3.Distance(monster.transform.position, slot);
                if (distance < minDistance)
                {
                    minDistance = distance; bestSlot = slot; foundSlot = true;
                }
            }
            if (foundSlot)
            {
                assignedSlotsData[monster] = bestSlot;
                monster.NotifyNewFormationSlotPosition(bestSlot);
                return bestSlot;
            }
        }
        
        if (!monstersWaitingForSlot.Contains(monster)) monstersWaitingForSlot.Enqueue(monster);
        return null;
    }

    public void ReleaseFormationSlot(MonsterAI monster)
    {
        if (monster == null) return;
        assignedSlotsData.Remove(monster);
    }
}