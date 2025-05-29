using System.Collections.Generic;
using UnityEngine;
public enum EquipmentType { Weapon, Armor, Head, Boots }

public enum SetItemType
{
    None     = 0,
    SetItem1 = 1 << 0,
    SetItem2 = 1 << 1,
    SetItem3 = 1 << 2,
    SetItem4 = 1 << 3,
    SetItem5 = 1 << 4
}
[CreateAssetMenu(fileName = "EquipmentData", menuName = "Scriptable Objects/EquipmentData")]
public class EquipmentDataSO : ItemBaseDataSO
{
    public new ItemType ItemType => ItemType.Equipment;
    
    public string EquipmentName;
    public EquipmentType EquipmentType;
    public GameObject ModelPrefab;

    public Dictionary<StatType, float> Stats = new Dictionary<StatType, float>();

    public SetItemType SetItemType;
    // public float MaxHealth;
    // public float AttackPower;
    // public float Defense;
    // public float Speed;
    // public float AttackSpeed;
    // public float CritChance;
    // public float CritDamage;
}
