using System;
using UnityEngine;
public enum ItemType { Equipment, Chip, Etc }
[Serializable]
[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public string Id;
    public string ItemName;
    public Sprite Icon;
    public ItemType ItemType;

    public string EquipmentDataId;
    public DataSO Data;
}
