using UnityEngine;

public class ItemDropManager : Singleton<ItemDropManager>
{
    // 1. 장비 아이템(Weapon, Head, Armor, Boots) 및 스킬 칩(Chip) 드랍
    public void DropEquipmentOrChip(Vector3 position)
    {
        // 예시: 랜덤으로 장비/칩 중 하나를 생성
        int rand = Random.Range(0, 5); // 0:Weapon, 1:Head, 2:Armor, 3:Boots, 4:Chip
        GameObject dropPrefab = null;

        switch (rand)
        {
            case 0:
                dropPrefab = GetRandomEquipmentPrefab(EquipmentType.Weapon);
                break;
            case 1:
                dropPrefab = GetRandomEquipmentPrefab(EquipmentType.Head);
                break;
            case 2:
                dropPrefab = GetRandomEquipmentPrefab(EquipmentType.Armor);
                break;
            case 3:
                dropPrefab = GetRandomEquipmentPrefab(EquipmentType.Boots);
                break;
            case 4:
                dropPrefab = GetRandomChipPrefab();
                break;
        }

        if (dropPrefab != null)
        {
            Instantiate(dropPrefab, position, Quaternion.identity);
        }
    }

    // 2. 기타(Etc) 아이템 드랍
    public void DropEtcItem(Vector3 position)
    {
        GameObject etcPrefab = GetRandomEtcPrefab();
        if (etcPrefab != null)
        {
            Instantiate(etcPrefab, position, Quaternion.identity);
        }
    }

    // 3. 골드 드랍
    public void DropGold(Vector3 position, int amount)
    {
        // 골드 프리팹에 amount를 전달하는 방식(예시)
        // GameObject goldPrefab = GetGoldPrefab();
        // if (goldPrefab != null)
        // {
        //     GameObject goldObj = Instantiate(goldPrefab, position, Quaternion.identity);
        //     Gold gold = goldObj.GetComponent<Gold>();
        //     if (gold != null)
        //     {
        //         gold.SetAmount(amount);
        //     }
        // }
    }

    // 아래는 프리팹을 랜덤으로 가져오는 예시 함수들 (직접 구현 필요)
    private GameObject GetRandomEquipmentPrefab(EquipmentType type) { /* ... */ return null; }
    private GameObject GetRandomChipPrefab() { /* ... */ return null; }
    private GameObject GetRandomEtcPrefab() { /* ... */ return null; }
    private GameObject GetGoldPrefab() { /* ... */ return null; }
}