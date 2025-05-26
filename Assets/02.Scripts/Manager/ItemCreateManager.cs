// using System.Collections.Generic;
// using UnityEngine;
//
// public class ItemCreateManager : Singleton<ItemCreateManager>
// {
//     public Sprite WeaponSprite;
//     public Sprite HeadSprite;
//     public Sprite ArmorSprite;
//     public Sprite BootsSprite;
//     public Sprite UseItemSprite;
//     public Sprite ChipSprite;
//
//     public GameObject UniqueSwordPrefab; // 추후 List 형태로 프리팹을 모두 가지고 있음
//     private int _itemIndex = 0;
//     private int _weaponIndex = 0;
//     
//     public Item CreateEquipment()
//     {
//         Item item = new Item();
//         item.Id = $"Item_{_itemIndex}";
//         item.ItemName = $"Weapon_{_weaponIndex}";
//         item.EquipmentDataId = $"{ItemType.Equipment.ToString()}.{EquipmentType.Weapon.ToString()}_{_weaponIndex.ToString()}"
//         _itemIndex++;
//         _weaponIndex++;
//
//         item.Icon = WeaponSprite;
//         item.ItemType = ItemType.Equipment;
//         EquipmentData data = new EquipmentData();
//         data.Id = $"EquipmentDataId_{EquipmentType.Weapon.ToString()}_{_weaponIndex}";
//         data.EquipmentName = "Katana"; // 추후 수정
//         data.ModelPrefab = UniqueSwordPrefab;
//         
//         Dictionary<StatType, float> stat = new Dictionary<StatType, float>();
//         foreach (StatType statType in System.Enum.GetValues(typeof(StatType)))
//         {
//             stat[statType] = 0.0f;
//         }
//         stat[StatType.AttackPower] = 100.0f;
//         stat[StatType.Speed] = 10.0f;
//         stat[StatType.AttackSpeed] = 10f;
//         stat[StatType.CritChance] = 20.0f;
//         stat[StatType.CritDamage] = 20.0f;
//         data.Stats = stat;
//         //data.Stats[StatType.AttackPower] = 100.0f; 
//         
//         item.EquipmentData = data;
//         
//         ChipData chipData = new ChipData();
//         chipData.SkillRange = 0.0f;
//         chipData.ReduceCooldown = 0.0f;
//         item.ChipData = chipData;
//         
//         return item;
//     }
// }

using System.Collections.Generic;
using UnityEngine;

public class ItemCreateManager : Singleton<ItemCreateManager>
{
    public Sprite WeaponSprite;
    public Sprite HeadSprite;
    public Sprite ArmorSprite;
    public Sprite BootsSprite;
    public Sprite UseItemSprite;
    public Sprite ChipSprite;

    public GameObject UniqueSwordPrefab;
    public GameObject ChipPrefab;
    public GameObject EtcPrefab;

    private int _itemIndex = 0;
    private int _weaponIndex = 0;
    private int _headIndex = 0;
    private int _armorIndex = 0;
    private int _bootsIndex = 0;
    private int _chipIndex = 0;
    private int _etcIndex = 0;

    // 공통 장비 생성 함수
    private Item CreateEquipment(EquipmentType type, Sprite icon, GameObject prefab, ref int index, string name = null)
    {
        Item item = new Item();
        item.Id = $"Item_{_itemIndex++}";
        item.ItemName = name ?? $"{type}_{index}";
        item.EquipmentDataId = $"{ItemType.Equipment}.{type}_{index++}";
        item.Icon = icon;
        item.ItemType = ItemType.Equipment;

        EquipmentData data = new EquipmentData();
        data.Id = $"EquipmentDataId_{type}_{index}";
        data.EquipmentName = name ?? type.ToString();
        data.ModelPrefab = prefab;

        Dictionary<StatType, float> stat = new Dictionary<StatType, float>();
        foreach (StatType statType in System.Enum.GetValues(typeof(StatType)))
        {
            stat[statType] = 0.0f;
        }
        // 예시: 타입별로 스탯 다르게 할당 가능
        if (type == EquipmentType.Weapon)
        {
            stat[StatType.AttackPower] = 100.0f;
        }
        if (type == EquipmentType.Head)
        {
            stat[StatType.MaxHealth] = 50.0f;
        }
        if (type == EquipmentType.Armor)
        {
            stat[StatType.Defense] = 30.0f;
        }
        if (type == EquipmentType.Boots)
        {
            stat[StatType.Speed] = 15.0f;
        }

        data.Stats = stat;
        item.EquipmentData = data;

        return item;
    }

    public Item CreateWeapon()
        => CreateEquipment(EquipmentType.Weapon, WeaponSprite, UniqueSwordPrefab, ref _weaponIndex, "Katana");

    public Item CreateHead()
        => CreateEquipment(EquipmentType.Head, HeadSprite, null, ref _headIndex);

    public Item CreateArmor()
        => CreateEquipment(EquipmentType.Armor, ArmorSprite, null, ref _armorIndex);

    public Item CreateBoots()
        => CreateEquipment(EquipmentType.Boots, BootsSprite, null, ref _bootsIndex);

    public Item CreateChip()
    {
        Item item = new Item();
        item.Id = $"Item_{_itemIndex++}";
        item.ItemName = $"Chip_{_chipIndex++}";
        item.Icon = ChipSprite;
        item.ItemType = ItemType.Chip;
        item.ChipData = new ChipData
        {
            SkillRange = 1.0f,
            ReduceCooldown = 0.2f
        };
        // ChipData에 필요한 값 추가
        return item;
    }

    public Item CreateEtc()
    {
        Item item = new Item();
        item.Id = $"Item_{_itemIndex++}";
        item.ItemName = $"Etc_{_etcIndex++}";
        item.Icon = UseItemSprite;
        item.ItemType = ItemType.Etc;
        // 필요시 EtcData 등 추가
        return item;
    }
}
