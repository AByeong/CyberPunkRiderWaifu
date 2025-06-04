using System;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

// 전역 Enum 선언
public enum DropItemType { Item, Etc, Gold }

public class ItemDropManager : Singleton<ItemDropManager>
{
    public ObjectPool Pool;
    // 등급 Enum
    private enum DropGrade { Normal, Rare, Unique }
    // 프리팹 순서대로 Enum
    private enum VFXType { Normal, Rare, Unique, Etc}

    private VFXType _prefabIndex = VFXType.Normal;


    // 각 타입별 VFX 프리팹
    protected override void Awake()
    {
        base.Awake();
        Pool = GetComponent<ObjectPool>();
    }

    private void Start()
    {
        if (Pool == null)
        {
            Debug.LogError("ItemDropManager에 Pool 없음");
            this.enabled = false; // 스크립트 비활성화
            return;
        }
    }

    // Enemy에서 DropItemType 리스트를 넘기면 처리
    public void DropItems(Dictionary<DropItemType, int> dropPlan, Vector3 origin, Vector3 forward)
    {
        Debug.Log("드랍");
        StartCoroutine(DropItemsCoroutine(dropPlan, origin, forward));
    }

    private IEnumerator DropItemsCoroutine(Dictionary<DropItemType, int> dropPlan, Vector3 origin, Vector3 forward)
    {
        // 루프 전에 미리 ground Y 계산
        Vector3 raycastOrigin = origin + Vector3.up * 2f; // 약간 위에서 쏘면 안정적
        float groundY = origin.y - 1f; // 기본값 (못 맞췄을 때 대비)

        if (Physics.Raycast(raycastOrigin, Vector3.down, out RaycastHit hit, 10f, LayerMask.GetMask("Ground")))
        {
            groundY = hit.point.y;
        }
        foreach (var kvp in dropPlan)
        {
            DropItemType type = kvp.Key;
            int count = kvp.Value;
            for (int i = 0; i < count; i++)
            {
                GameObject vfx = null;
                if (type == DropItemType.Item)
                {
                    DropGrade grade = GetRandomGrade();
                    vfx = GetVFXByTypeAndGrade(type, grade);
                }
                else
                {
                    vfx = GetVFXByTypeAndGrade(type, DropGrade.Normal); // 등급 무시, 단일 VFX
                }

                Vector3 randomDir = (Quaternion.Euler(0, Random.Range(-30f, 30f), 0) * forward).normalized;
                Vector3 spawnPos = origin + randomDir * Random.Range(0.5f, 1.5f) + Vector3.up * 0.5f;

                spawnPos.y = groundY;
                if (vfx != null)
                {
                    
                    GameObject fx = Pool.GetObject((int)_prefabIndex);
                    fx.transform.position = origin;
                    fx.transform.rotation = Quaternion.identity;
                    fx.GetComponent<ItemObject>().Init(type);
                    fx.transform.DOJump(spawnPos, 0.7f, 1, 0.4f).SetEase(Ease.OutQuad);
                }
                yield return new WaitForSeconds(0.3f);
            }
        }
    }

    private DropGrade GetRandomGrade()
    {
        float rand = Random.value;
        if (rand < 0.7f)
        {
            _prefabIndex = VFXType.Normal;
            return DropGrade.Normal;
        }
        else if (rand < 0.95f)
        {
            _prefabIndex = VFXType.Rare;
            return DropGrade.Rare;
        }
        else
        {
            _prefabIndex = VFXType.Unique;
            return DropGrade.Unique;
        }
    }

    private GameObject GetVFXByTypeAndGrade(DropItemType type, DropGrade grade)
    {
        switch (type)
        {
            case DropItemType.Item:
                switch (grade)
                {
                    case DropGrade.Normal: return Pool.GetObject((int)VFXType.Normal);
                    case DropGrade.Rare: return Pool.GetObject((int)VFXType.Rare);
                    case DropGrade.Unique: return Pool.GetObject((int)VFXType.Unique);
                }
                break;
            case DropItemType.Etc:
                return Pool.GetObject((int)VFXType.Etc);
            case DropItemType.Gold:
                return Pool.GetObject((int)VFXType.Etc);
        }
        return null;
    }
}