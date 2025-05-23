using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

// 전역 Enum 선언
public enum DropItemType { Equipment, Chip, Etc, Gold }

public class ItemDropManager : Singleton<ItemDropManager>
{
    // 등급 Enum
    private enum DropGrade { Normal, Rare, Unique }

    // 각 타입별 VFX 프리팹
    public GameObject EquipmentNormalVFX;
    public GameObject EquipmentRareVFX;
    public GameObject EquipmentUniqueVFX;
    public GameObject ChipVFX;
    public GameObject EtcVFX;
    public GameObject GoldVFX;

    // Enemy에서 DropItemType 리스트를 넘기면 처리
    public void DropItems(List<DropItemType> dropTypes, Vector3 origin, Vector3 forward)
    {
        int dropCount = Random.Range(1, 4); // 1~3개 랜덤
        StartCoroutine(DropItemsCoroutine(dropTypes, dropCount, origin, forward));
    }

    private IEnumerator DropItemsCoroutine(List<DropItemType> dropTypes, int count, Vector3 origin, Vector3 forward)
    {
        for (int i = 0; i < count; i++)
        {
            // 드랍할 타입 랜덤 선택
            DropItemType type = dropTypes[Random.Range(0, dropTypes.Count)];
            DropGrade grade = GetRandomGrade();
            GameObject vfx = GetVFXByTypeAndGrade(type, grade);

            // 퍼지는 위치 계산
            Vector3 randomDir = (Quaternion.Euler(0, Random.Range(-30f, 30f), 0) * forward).normalized;
            Vector3 spawnPos = origin + randomDir * Random.Range(0.5f, 1.5f) + Vector3.up * 0.5f;

            if (vfx != null)
            {
                GameObject fx = Instantiate(vfx, origin, Quaternion.identity);
                fx.transform.DOJump(spawnPos, 0.7f, 1, 0.4f).SetEase(Ease.OutQuad);
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    private DropGrade GetRandomGrade()
    {
        float rand = Random.value;
        if (rand < 0.7f) return DropGrade.Normal;
        if (rand < 0.95f) return DropGrade.Rare;
        return DropGrade.Unique;
    }

    private GameObject GetVFXByTypeAndGrade(DropItemType type, DropGrade grade)
    {
        switch (type)
        {
            case DropItemType.Equipment:
                switch (grade)
                {
                    case DropGrade.Normal: return EquipmentNormalVFX;
                    case DropGrade.Rare: return EquipmentRareVFX;
                    case DropGrade.Unique: return EquipmentUniqueVFX;
                }
                break;
            case DropItemType.Chip:
                return ChipVFX;
            case DropItemType.Etc:
                return EtcVFX;
            case DropItemType.Gold:
                return GoldVFX;
        }
        return null;
    }
}