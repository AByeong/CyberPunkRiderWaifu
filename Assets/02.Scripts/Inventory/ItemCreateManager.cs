
using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemCreateManager : Singleton<ItemCreateManager>
{
    // public Sprite WeaponSprite;
    // public Sprite HeadSprite;
    // public Sprite ArmorSprite;
    // public Sprite BootsSprite;
    // public Sprite UseItemSprite;
    // public Sprite ChipSprite;
    //
    // public GameObject UniqueSwordPrefab;
    // public GameObject ChipPrefab;
    // public GameObject EtcPrefab;

    public EquipmentDataSO NormalWeaponEquipmentDataSO;
    public EquipmentDataSO RareWeaponEquipmentDataSO;
    public EquipmentDataSO UniqueWeaponEquipmentDataSO;
    public EquipmentDataSO HeadEquipmentDataSO;
    public EquipmentDataSO ArmorEquipmentDataSO;
    public EquipmentDataSO BootsEquipmentDataSO;
    //Chip도 더 좋은 칩 구분할거면 추가
    public ChipDataSO ChipDataSO;
    
    public ItemBaseDataSO ItemBaseDataSO;
    
    private int _itemIndex = 0;
    private int _weaponIndex = 0;
    private int _headIndex = 0;
    private int _armorIndex = 0;
    private int _bootsIndex = 0;
    private int _chipIndex = 0;
    private int _etcIndex = 0;

    // 공통 장비 생성 함수
    private Item CreateEquipment(EquipmentType type, int rarity = 0)
    {
        // EquipmentDataSO.Id = $"Equipment.{type}_{index++}";
        // EquipmentDataSO.ItemName = name ?? $"{type}_{index}";
        // EquipmentDataSO.Icon = icon;
        //
        // EquipmentDataSO.EquipmentName = name ?? type.ToString();
        // EquipmentDataSO.EquipmentType = type;
        // EquipmentDataSO.ModelPrefab = prefab;
        Item item = null;
        switch (type)
        {
            case EquipmentType.Weapon:
                if(rarity == 0)
                    item = new Item(NormalWeaponEquipmentDataSO);
                else if(rarity == 1)
                    item = new Item(RareWeaponEquipmentDataSO);
                else if (rarity == 2)
                    item = new Item(UniqueWeaponEquipmentDataSO);
                else
                    Debug.LogError("무기 레어도는 0~2까지만");
                break;
            case EquipmentType.Armor:
                item = new Item(ArmorEquipmentDataSO);
                break;
            case EquipmentType.Head:
                item = new Item(HeadEquipmentDataSO);
                break;
            case EquipmentType.Boots:
                item = new Item(BootsEquipmentDataSO);
                break;
            default:
                Debug.LogError("무기 생성 Type이 잘못됐음");
                break;
        }

        // 예시: 타입별로 스탯 다르게 할당 가능
        if (type == EquipmentType.Weapon)
        {
            item.AttackPower = 100.0f;
        }
        if (type == EquipmentType.Head)
        {
            item.MaxHealth = 50.0f;
        }
        if (type == EquipmentType.Armor)
        {
            item.Defense = 30.0f;
        }
        if (type == EquipmentType.Boots)
        {
            item.Speed = 15.0f;
        }

        return item;
    }

    public Item CreateWeapon()
        => CreateEquipment(EquipmentType.Weapon);

    public Item CreateHead()
        => CreateEquipment(EquipmentType.Head);

    public Item CreateArmor()
        => CreateEquipment(EquipmentType.Armor);

    public Item CreateBoots()
        => CreateEquipment(EquipmentType.Boots);

    public Item CreateChip(int rarity = 0)
    {
        Item item = null;
        if(rarity == 0)
             item = new Item(ChipDataSO);
        else if(rarity == 1)
             item = new Item(ChipDataSO);
        else if(rarity == 2)
             item = new Item(ChipDataSO);

        return item;
        //Item item = new Item(InventoryManager.Instance.ChipSODatas[랜덤]), null);
        //InventoryManager.Add(item);
    }

    public Item CreateEtc()
    {
        Item item = new Item(ItemBaseDataSO);
        // ItemBaseDataSO itemBaseDataSo = new ItemBaseDataSO();
        // itemBaseDataSo.Id = $"Item_{_itemIndex++}";
        // itemBaseDataSo.ItemName = $"Etc_{_etcIndex++}";
        // itemBaseDataSo.Icon = UseItemSprite;
        // itemBaseDataSo.ItemType = ItemType.Etc;
        // 필요시 EtcData 등 추가

        return item;
    }
}
