
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
    private ItemBaseDataSO CreateEquipment(EquipmentType type, Sprite icon, GameObject prefab, ref int index, string name = null)
    {
        ItemBaseDataSO itemBaseDataSo = new ItemBaseDataSO();
        itemBaseDataSo.Id = $"Item_{_itemIndex++}";
        itemBaseDataSo.ItemName = name ?? $"{type}_{index}";
        itemBaseDataSo.Icon = icon;
        itemBaseDataSo.ItemType = ItemType.Equipment;

        itemBaseDataSo.Id = $"{ItemType.Equipment}.{type}_{index++}";
        EquipmentDataSO dataSo = new EquipmentDataSO();
        dataSo.Id = $"EquipmentDataId_{type}_{index}";
        dataSo.EquipmentName = name ?? type.ToString();
        dataSo.ModelPrefab = prefab;

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

        dataSo.Stats = stat;
        itemBaseDataSo = dataSo;

        return itemBaseDataSo;
    }

    public ItemBaseDataSO CreateWeapon()
        => CreateEquipment(EquipmentType.Weapon, WeaponSprite, UniqueSwordPrefab, ref _weaponIndex, "Katana");

    public ItemBaseDataSO CreateHead()
        => CreateEquipment(EquipmentType.Head, HeadSprite, null, ref _headIndex);

    public ItemBaseDataSO CreateArmor()
        => CreateEquipment(EquipmentType.Armor, ArmorSprite, null, ref _armorIndex);

    public ItemBaseDataSO CreateBoots()
        => CreateEquipment(EquipmentType.Boots, BootsSprite, null, ref _bootsIndex);

    public ItemBaseDataSO CreateChip()
    {
        return null;


        
        //Item item = new Item(InventoryManager.Instance.ChipSODatas[랜덤]), null);
        //InventoryManager.Add(item);
    }

    public ItemBaseDataSO CreateEtc()
    {
        ItemBaseDataSO itemBaseDataSo = new ItemBaseDataSO();
        itemBaseDataSo.Id = $"Item_{_itemIndex++}";
        itemBaseDataSo.ItemName = $"Etc_{_etcIndex++}";
        itemBaseDataSo.Icon = UseItemSprite;
        itemBaseDataSo.ItemType = ItemType.Etc;
        // 필요시 EtcData 등 추가
        return itemBaseDataSo;
    }
}
