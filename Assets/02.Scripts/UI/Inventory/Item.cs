using System;
using UnityEngine;
public enum ItemType { Equipment, Chip, Etc }

[Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "Item", order = 1)]
public class Item : ScriptableObject
{
    public string Id;
    public string ItemName;
    public Sprite Icon;

    public ItemType Type;
    public DataData Data;
}
