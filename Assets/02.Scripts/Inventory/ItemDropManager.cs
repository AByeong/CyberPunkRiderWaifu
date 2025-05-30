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
    public GameObject EquipmentNormalVFX;
    public GameObject EquipmentRareVFX;
    public GameObject EquipmentUniqueVFX;
    public GameObject EtcVFX;
    public GameObject GoldVFX;

    
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

                if (vfx != null)
                {
                    
                    GameObject fx = Pool.GetObject((int)_prefabIndex);
                    fx.transform.position = origin;
                    fx.transform.rotation = Quaternion.identity;
                    fx.GetComponent<ItemObject>().Init();
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
                    case DropGrade.Normal: return EquipmentNormalVFX;
                    case DropGrade.Rare: return EquipmentRareVFX;
                    case DropGrade.Unique: return EquipmentUniqueVFX;
                }
                break;
            case DropItemType.Etc:
                return EtcVFX;
            case DropItemType.Gold:
                return GoldVFX;
        }
        return null;
    }
}