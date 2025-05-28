using System;
using UnityEngine;

public class Item
{
    public readonly ItemBaseDataSO Data;

    private int _slotIndex = -1;
    private SlotType _slotType = SlotType.Inventory;
    public int SlotIndex => _slotIndex;
    public SlotType SlotType => _slotType;

    public void SetSlotIndex(SlotType slotType, int slotIndex)
    {
        _slotType = slotType;
        _slotIndex = slotIndex;
    }
    // Equipment 옵션
    public float MaxHealth;
    public float AttackPower;
    public float Defense;
    public float Speed;
    public float AttackSpeed;
    public float CritChance;
    public float CritDamage;

    // Chip 옵션
    public float SkillRange;
    public float ReduceCooldown;

    public Item(ItemBaseDataSO data,  ItemSaveData saveData = null)
    {
        this.Data = data;
        
        if (saveData != null)
        {
            _slotIndex = saveData.SlotIndex;
            if (saveData is EquipmentSaveData equipmentSaveData)
            {
                MaxHealth = equipmentSaveData.MaxHealth;
                AttackPower = equipmentSaveData.AttackPower;
                Defense = equipmentSaveData.Defense;
                Speed = equipmentSaveData.Speed;
                AttackSpeed = equipmentSaveData.AttackSpeed;
                CritChance = equipmentSaveData.CritChance;
                CritDamage = equipmentSaveData.CritDamage;
            }
            else if (saveData is ChipSaveData chipSaveData)
            {
                SkillRange = chipSaveData.SkillRange;
                ReduceCooldown = chipSaveData.ReduceCooldown;
            }
        }
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
                CritDamage = CritDamage
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

        saveData.ItemType = Data.ItemType;
        saveData.ItemSOIndex = soIndex;
        saveData.SlotIndex = _slotIndex;
        saveData.SlotType = _slotType;

        return saveData;
    }
}
    
