using System;
using UnityEngine;
public enum ItemType { Equipment, Chip, Etc }
[Serializable]
public class Item
{
    public string Id;
    public string ItemName;
    public Sprite Icon;
}
