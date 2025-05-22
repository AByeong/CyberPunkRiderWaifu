using System;
using UnityEngine;
public enum ItemType { Equipment, Chip, Etc }
[Serializable]
public class Item
{
    public string Id;
    public string ItemName;
    public Sprite Icon;
    public ItemType ItemType;

    public string EquipmentDataId;
    public EquipmentData EquipmentData;
    public ChipData ChipData;
}
