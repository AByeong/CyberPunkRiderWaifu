using UnityEngine;

public class Item
{
    public readonly ItemBaseDataSO Data;

    private int _slotIndex = -1;
    public int SlotIndex => _slotIndex;

    public void SetSlotIndex(int slotIndex)
    {
        _slotIndex = slotIndex;
    }
    
    
    // public float MaxHealth;
    // public float AttackPower;
    // public float Defense;
    // public float Speed;
    // public float AttackSpeed;
    // public float CritChance;
    // public float CritDamage;

    public Item(ItemBaseDataSO data,  ItemSaveData saveData = null)
    {
        this.Data = data;
        
        if (saveData != null)
        {
            _slotIndex = saveData.SlotIndex;
        }
        
    }
}
    
