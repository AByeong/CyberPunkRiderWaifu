using System;
using UnityEngine;
public enum ItemType { Equipment, Chip, Etc }
public enum ItemRarity { Normal, Rare, Unique}
[Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/ItemData")]
public class ItemBaseDataSO : ScriptableObject
{
    public string Id;
    public ItemType ItemType;
    public ItemRarity ItemRarity;
    public string ItemName;
    public Sprite Icon;
}



/*
 *  Item Class가 ItemData(SO)와 ItemSaveData를 가지고있음
 * 
 */