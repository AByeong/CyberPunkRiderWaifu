using System;
using System.Collections.Generic;
using UnityEngine;
public class ItemCreateManager : Singleton<ItemCreateManager>
{
    public EquipmentDataSO NormalArmorEquipmentDataSO;
    public EquipmentDataSO NormalBootsEquipmentDataSO;
    public EquipmentDataSO NormalHeadEquipmentDataSO;
    public EquipmentDataSO NormalWeaponEquipmentDataSO;
    
    public EquipmentDataSO RareArmorEquipmentDataSO;
    public EquipmentDataSO RareBootsEquipmentDataSO;
    public EquipmentDataSO RareHeadEquipmentDataSO;
    public EquipmentDataSO RareWeaponEquipmentDataSO;
    
    public EquipmentDataSO UniqueArmorEquipmentDataSO;
    public EquipmentDataSO UniqueBootsEquipmentDataSO;
    public EquipmentDataSO UniqueHeadEquipmentDataSO;
    public EquipmentDataSO UniqueWeaponEquipmentDataSO;
    //Chip도 더 좋은 칩 구분할거면 추가
    public ChipDataSO NormalChipDataSO;
    public ChipDataSO RareChipDataSO;
    public ChipDataSO UniqueChipDataSO;
    
    private static readonly System.Random _random = new System.Random();
   
    // 공통 장비 생성 함수
    private Item CreateEquipment(EquipmentType type, ItemRarity rarity = ItemRarity.Normal)
    {
        Item item = null;
        // 예시: 타입별로 스탯 다르게 할당 가능
        // 무기 스탯 -> AttackPower, AttackSpeed, CritChance, CritDamage
        // 몸통 방어구 스탯 -> Defense
        // 머리 방어구 스탯 -> MaxHealth
        // 신발 방어구 스탯 -> Speed
        // 각 장비들은 유니크면 세트효과 랜덤하게 지정됨
        // TODO : 유니크 무기는 스킬의 추가효과 만들어야함
        string uniqueID = Guid.NewGuid().ToString();
        
        switch (type)
        {
            case EquipmentType.Weapon:
                if(rarity == ItemRarity.Normal)
                {
                    item = new Item(NormalWeaponEquipmentDataSO, 3);
                    item.AttackPower = MakeRandomStat(90.0f, 110.0f);
                    item.AttackSpeed = MakeRandomStat(0.0f, 0.5f);
                    item.CritChance = MakeRandomStat(0.01f, 0.2f);
                    item.CritDamage = MakeRandomStat(0.5f, 1.0f);
                }
                else if(rarity == ItemRarity.Rare)
                {
                    item = new Item(RareWeaponEquipmentDataSO, 7);
                    item.AttackPower = MakeRandomStat(130.0f, 180.0f);
                    item.AttackSpeed = MakeRandomStat(0.2f, 1.0f);
                    item.CritChance = MakeRandomStat(0.1f, 0.3f);
                    item.CritDamage = MakeRandomStat(0.8f, 2.0f);
                }
                else if (rarity == ItemRarity.Unique)
                {
                    item = new Item(UniqueWeaponEquipmentDataSO, 11);
                    item.AttackPower = MakeRandomStat(200.0f, 400.0f);
                    item.AttackSpeed = MakeRandomStat(0.5f, 2.0f);
                    item.CritChance = MakeRandomStat(0.2f, 0.5f);
                    item.CritDamage = MakeRandomStat(1.0f, 3.0f);
                    item.SetItemType = MakeRandomSetItemType();
                }

                break;
            case EquipmentType.Armor:
                if(rarity == ItemRarity.Normal)
                {
                    item = new Item(NormalArmorEquipmentDataSO, 0);
                    item.Defense = MakeRandomStat(0.01f, 0.1f);
                }
                else if(rarity == ItemRarity.Rare)
                {
                    item = new Item(RareArmorEquipmentDataSO, 4);
                    item.Defense = MakeRandomStat(0.1f, 0.3f);

                }
                else if (rarity == ItemRarity.Unique)
                {
                    item = new Item(UniqueArmorEquipmentDataSO, 8);
                    item.Defense = MakeRandomStat(0.2f, 0.5f);
                    item.SetItemType = MakeRandomSetItemType();
                }
                break;
            case EquipmentType.Head:
                if(rarity == ItemRarity.Normal)
                {
                    item = new Item(NormalHeadEquipmentDataSO,2);
                    item.MaxHealth = MakeRandomStat(100.0f, 250.0f);
                }
                else if(rarity == ItemRarity.Rare)
                {
                    item = new Item(RareHeadEquipmentDataSO,6);
                    item.MaxHealth = MakeRandomStat(200.0f, 400.0f);

                }
                else if (rarity == ItemRarity.Unique)
                {
                    item = new Item(UniqueHeadEquipmentDataSO,10);
                    item.MaxHealth = MakeRandomStat(300.0f, 800.0f);
                    item.SetItemType = MakeRandomSetItemType();
                }
                break;
            case EquipmentType.Boots:
                if(rarity == ItemRarity.Normal)
                {
                    item = new Item(NormalBootsEquipmentDataSO, 1);
                    item.Speed = MakeRandomStat(1.0f, 2.0f);
                }
                else if(rarity == ItemRarity.Rare)
                {
                    item = new Item(RareBootsEquipmentDataSO,5);
                    item.Speed = MakeRandomStat(2.0f, 5.0f);
                }
                else if (rarity == ItemRarity.Unique)
                {
                    item = new Item(UniqueBootsEquipmentDataSO,9);
                    item.Speed = MakeRandomStat(3.0f, 8.0f);
                    item.SetItemType = MakeRandomSetItemType();
                }
                break;
            default:
                Debug.LogError("무기 생성 Type이 잘못됐음");
                break;
        }
        MakeRandomCubeStats(item);
        item.Id = uniqueID;
        return item;
    }

    private float MakeRandomStat(float min, float max)
    {
        if (min >= max)
        {
            Debug.LogWarning($"MakeRandomStat: 최소값({min})이 최대값({max})보다 크거나 같습니다.");
            return min;
        }
        // 1. 난수 생성 (0.0 ~ 1.0 사이)
        double randomValue = _random.NextDouble();
        // 2. 전체 범위 길이 계산
        float range = max - min; // 예: 120 - 80 = 40
        // 3. 랜덤값을 범위 길이에 곱해서 오프셋 계산
        float offset = (float)(range * randomValue); // 예: 40 * 0.62 = 24.8
        // 4. 최소값에 오프셋을 더해 최종값 생성
        float finalValue = min + offset; // 예: 80 + 24.8 = 104.8

        return finalValue;
    }
    private void MakeRandomCubeStats(Item item)
    {
        for(int i = 0; i < 3; i++)
        {
            float value = 0.0f;
            StatType statType = MakeRandomStatType();
            switch (item.Data.ItemRarity)
            {
                case ItemRarity.Normal:
                    value = 0.09f;
                    break;
                case ItemRarity.Rare:
                    value = 0.12f;
                    break;
                case ItemRarity.Unique:
                    value = 0.21f;
                    break;
            }
            item.CubeStats.Add(new CubeStat(statType, value));
        }
    }
    private StatType MakeRandomStatType()
    {
        int randomNumber = UnityEngine.Random.Range(0, 140);

        if (randomNumber < 20)
        {
            return StatType.MaxHealth;
        }
        else if (randomNumber < 40)
        {
            return StatType.AttackPower;
        }
        else if (randomNumber < 60)
        {
            return StatType.Defense;
        }
        else if (randomNumber < 80)
        {
            return StatType.Speed;
        }
        else if (randomNumber < 100)
        {
            return StatType.AttackSpeed;
        }
        else if (randomNumber < 120)
        {
            return StatType.CritChance;
        }
        else
        {
            return StatType.CritDamage;
        }
    }

    private SetItemType MakeRandomSetItemType()
    {
        // None은 제외하고 SetItem1~SetItem5 사이에서 랜덤 선택
        Array values = Enum.GetValues(typeof(SetItemType));
        List<SetItemType> selectable = new List<SetItemType>();

        foreach (SetItemType val in values)
        {
            if (val != SetItemType.None)
                selectable.Add(val);
        }

        int index = UnityEngine.Random.Range(0, selectable.Count);
        return selectable[index];
    }

    public Item CreateWeapon(ItemRarity rarity =  ItemRarity.Normal)
        => CreateEquipment(EquipmentType.Weapon, rarity);

    public Item CreateHead(ItemRarity rarity =  ItemRarity.Normal)
        => CreateEquipment(EquipmentType.Head, rarity);

    public Item CreateArmor(ItemRarity rarity =  ItemRarity.Normal)
        => CreateEquipment(EquipmentType.Armor, rarity);

    public Item CreateBoots(ItemRarity rarity =  ItemRarity.Normal)
        => CreateEquipment(EquipmentType.Boots, rarity);

    public Item CreateChip(ItemRarity rarity = ItemRarity.Normal)
    {
        Item item = null;
        string uniqueID = Guid.NewGuid().ToString();
        
        if(rarity == ItemRarity.Normal)
             item = new Item(NormalChipDataSO,12 );
        else if(rarity == ItemRarity.Rare)
             item = new Item(RareChipDataSO,13);
        else if(rarity == ItemRarity.Unique)
             item = new Item(UniqueChipDataSO, 14);
        item.Id = uniqueID;
        
        return item;
    }
    public ItemBaseDataSO GetItemDataByIndex(int index)
    {
        Debug.Log($"GetItemDataByIndex: {index}");
        switch (index)
        {
            case 0: return NormalArmorEquipmentDataSO;
            case 1: return NormalBootsEquipmentDataSO;
            case 2: return NormalHeadEquipmentDataSO;
            case 3: return NormalWeaponEquipmentDataSO;

            case 4: return RareArmorEquipmentDataSO;
            case 5: return RareBootsEquipmentDataSO;
            case 6: return RareHeadEquipmentDataSO;
            case 7: return RareWeaponEquipmentDataSO;

            case 8: return UniqueArmorEquipmentDataSO;
            case 9: return UniqueBootsEquipmentDataSO;
            case 10: return UniqueHeadEquipmentDataSO;
            case 11: return UniqueWeaponEquipmentDataSO;

            case 12: return NormalChipDataSO;
            case 13: return RareChipDataSO;
            case 14: return UniqueChipDataSO;

            default:
                Debug.LogWarning($"Invalid index {index} passed to GetItemDataByIndex()");
                return null;
        }
    }
}
