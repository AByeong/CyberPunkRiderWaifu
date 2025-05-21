using UnityEngine;

public enum ItemType {Equipment, Consumable, Etc}
[System.Serializable]
public class Item
{
    public string ItemName;
    public Sprite Icon;
    public ItemType ItmeType;
}
