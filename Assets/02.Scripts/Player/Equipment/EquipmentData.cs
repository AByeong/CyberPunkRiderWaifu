using System.Collections.Generic;
using UnityEngine;
public enum EquipmentType { Weapon, Armor, Head, Boots }

[CreateAssetMenu(fileName = "EquipmentData", menuName = "Scriptable Objects/EquipmentData")]
public class EquipmentData : ScriptableObject
{
    public string EquipmentName;
    public EquipmentType EquipmentType;
    
    public int MaxHealth;
    public int AttackPower;
    public int Defense;
    public float Speed;
    public float AttackSpeed;
    public float CritChance;
    public float CritDamage;
    public GameObject ModelPrefab;
}
