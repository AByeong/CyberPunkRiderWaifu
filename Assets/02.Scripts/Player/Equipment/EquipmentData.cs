using UnityEngine;
public enum EquipmentType { Weapon, Armor, Head, Boots }

[CreateAssetMenu(fileName = "EquipmentData", menuName = "Scriptable Objects/EquipmentData")]
public class EquipmentData : ScriptableObject
{
    public string equipmentName;
    public EquipmentType slot;
    
    public int maxHealth;
    public int attackPower;
    public int defense;
    public float speed;
    public float attackSpeed;
    public float critChance;
    public float critDamage;

    public GameObject modelPrefab;
}
