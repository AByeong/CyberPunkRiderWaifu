using System;
using System.Collections.Generic;
using UnityEngine;

public class CubeStat
{
    public StatType StatType;
    public float Value;

    public CubeStat(StatType statType, float value)
    {
        StatType = statType;
        Value = value;
    }
}

// [Serializable]
// public class ChipItemData
// {
//     private string Id;
//     
//     public ItemRarity ItemRarity;
//
//     public float SkillRange;
//     public float ReduceCooldow;
//
// }
[Serializable]
public class Item
{
    public string Id;
    public int SoIndex;
    public ItemBaseDataSO Data;

    public bool IsEquipped;

    private int _slotIndex = -1;
    private SlotType _slotType = SlotType.Inventory;
    public int SlotIndex => _slotIndex;
    public SlotType SlotType => _slotType;

    public EquipmentType EquipmentType
    {
        get
        {
            if (Data is EquipmentDataSO equipmentData)
                return equipmentData.EquipmentType;
            return EquipmentType.Weapon; // 기본값
        }
    }


    public void SetSlotIndex(SlotType slotType, int slotIndex)
    {
        _slotType = slotType;
        _slotIndex = slotIndex;

        // 장착
        if (_slotType == SlotType.Equipment)
        {
            IsEquipped = true;
        }

        // 장착 해제
        if (_slotType == SlotType.Inventory)
        {
            IsEquipped = false;
        }
    }

    // Equipment 옵션
    public float MaxHealth = 0.0f;
    public float AttackPower = 0.0f;
    public float Defense = 0.0f;
    public float Speed = 0.0f;
    public float AttackSpeed = 0.0f;
    public float CritChance = 0.0f;
    public float CritDamage = 0.0f;

    public SetItemType SetItemType;
    public ItemSaveData SaveData;
    //public Dictionary<StatType, float> CubeStats = new Dictionary<StatType, float>();
    public List<CubeStat> CubeStats = new List<CubeStat>();

    // Chip 옵션
    public float SkillRange
    {
        get
        {
            if (Data is ChipDataSO chipData)
                return chipData.SkillRange;
            return 0.0f; // 기본값
        }
        set { }
    }

    public float ReduceCooldown
    {
        get
        {
            if (Data is ChipDataSO chipData)
                return chipData.ReduceCooldown;
            return 0.0f; // 기본값
        }
        set { }
    }

    public Item(ItemBaseDataSO data = null, int soIndex = -1, ItemSaveData saveData = null)
    {
        this.Data = data;
        SoIndex = soIndex;
        if (saveData != null)
        {
            _slotIndex = saveData.SlotIndex;
            if (data.ItemType == ItemType.Equipment)
            {
                EquipmentSaveData equipmentSaveData = saveData as EquipmentSaveData;
                MaxHealth = equipmentSaveData.MaxHealth;
                AttackPower = equipmentSaveData.AttackPower;
                Defense = equipmentSaveData.Defense;
                Speed = equipmentSaveData.Speed;
                AttackSpeed = equipmentSaveData.AttackSpeed;
                CritChance = equipmentSaveData.CritChance;
                CritDamage = equipmentSaveData.CritDamage;
                SetItemType = equipmentSaveData.SetItemType;
                CubeStats = equipmentSaveData.CubeStats;
            }
            else if (saveData is ChipSaveData chipSaveData)
            {
                SkillRange = chipSaveData.SkillRange;
                ReduceCooldown = chipSaveData.ReduceCooldown;
            }
        }

    }

    public void Save()
    {
        string json = JsonUtility.ToJson(ToSaveData(SoIndex));
        PlayerPrefs.SetString("Key" + Id.ToString(), json);
        PlayerPrefs.Save();
    }

    public ItemSaveData ToSaveData(int soIndex)
    {
        ItemSaveData saveData = null;

        if (Data.ItemType == ItemType.Equipment)
        {
            saveData = new EquipmentSaveData
            {
                MaxHealth = MaxHealth,
                AttackPower = AttackPower,
                Defense = Defense,
                Speed = Speed,
                AttackSpeed = AttackSpeed,
                CritChance = CritChance,
                CritDamage = CritDamage,
                SetItemType = SetItemType
            };
        }
        else if (Data.ItemType == ItemType.Chip)
        {
            saveData = new ChipSaveData
            {
                SkillRange = SkillRange,
                ReduceCooldown = ReduceCooldown
            };
        }
        else
        {
            saveData = new ItemSaveData(); // 예외 방지
        }

        saveData.Id = Id;
        saveData.ItemType = Data.ItemType;
        saveData.SOIndex = soIndex;
        saveData.SlotIndex = _slotIndex;
        saveData.SlotType = _slotType;

        return saveData;
    }

    public bool Load(string Id)
    {
        if (PlayerPrefs.HasKey("Key" + Id))
        {
            Debug.Log("ItemLoad" + Id);
            string json = PlayerPrefs.GetString("Key" + Id);
            SaveData =  JsonUtility.FromJson<ItemSaveData>(json);
            Data = ItemCreateManager.Instance.GetItemDataByIndex(SaveData.SOIndex);
            Id = SaveData.Id;
            Data.ItemType = SaveData.ItemType;
            SoIndex = SaveData.SOIndex;
            _slotIndex = SaveData.SlotIndex;
            _slotType = SaveData.SlotType;
            SetSlotIndex(_slotType,_slotIndex);
            return true;
        }
        return false;
    }

}
    
