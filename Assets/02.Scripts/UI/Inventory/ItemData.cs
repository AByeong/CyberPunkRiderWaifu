using System;
using UnityEngine;
public enum ItemType { Equipment, Chip, Etc }
[Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public string Id;
    public string ItemName;
    public Sprite Icon;
    public ItemType ItemType;

    public string EquipmentDataId;
    public DataSO Data;
}
