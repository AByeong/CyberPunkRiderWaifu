using System.Collections.Generic;
using UnityEngine;
public enum EquipmentType { Weapon, Armor, Head, Boots }

[CreateAssetMenu(fileName = "EquipmentData", menuName = "Scriptable Objects/EquipmentData")]
public class EquipmentDataSO : ItemBaseDataSO
{
    public new ItemType ItemType => ItemType.Equipment;
    
    public string EquipmentName;
    public EquipmentType EquipmentType;
    public GameObject ModelPrefab;

    public Dictionary<StatType, float> Stats = new Dictionary<StatType, float>();
    // public float MaxHealth;
    // public float AttackPower;
    // public float Defense;
    // public float Speed;
    // public float AttackSpeed;
    // public float CritChance;
    // public float CritDamage;
}
