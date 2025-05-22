using UnityEngine;

public enum ItemType {Equipment, Consumable, Etc}
[System.Serializable]
public class Item
{
    public string Id;
    public string ItemName;
    public Sprite Icon;
    public ItemType ItemType;

    public string EquipmentDataId;
    public EquipmentData EquipmentData;
}
