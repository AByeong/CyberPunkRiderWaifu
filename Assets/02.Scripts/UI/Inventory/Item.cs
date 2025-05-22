using System;
using UnityEngine;
public enum ItemType { Equipment, Chip, Etc }
[Serializable]
public class Item
{
    public string ItemName;
    public Sprite Icon;
    public ItemType ItmeType;
    public EquipmentData EquipmentData;
}
