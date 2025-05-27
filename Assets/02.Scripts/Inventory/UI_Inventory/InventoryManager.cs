using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
// public class ItemData
// {
//     public string itemId;
//     public int slotIndex;
// }
//
// [System.Serializable]
// public class ItemDataList
// {
//     public List<ItemData> items = new List<ItemData>();
// }
public class InventoryManager : Singleton<InventoryManager>
{
    public List<ItemBaseDataSO> soDatas;

    private List<Item> _items = new List<Item>();
    public List<Item> Items => _items;
    
    public Action OnDataChanged;

    public void Start()
    {
        Load();
    }

    public void Add(Item item)
    {
        _items.Add(item);

        OnDataChanged?.Invoke();
    }


    private void Save()
    {
        ItemSaveDataList dataList = new ItemSaveDataList();
        foreach (var item in _items)
        {
            var data = new ItemSaveData();
            data.ItemType = item.Data.ItemType;
            data.SlotIndex = item.SlotIndex;
            data.ItemIndex = soDatas.IndexOf(item.Data);
            
            dataList.SaveDatas.Add(data);
        }
        
        PlayerPrefs.SetString("Inventory", JsonUtility.ToJson(dataList));
    }

    private void Load()
    {
        string stringData =  PlayerPrefs.GetString("Inventory", "");
        if (string.IsNullOrEmpty(stringData))
        {
            return;
        }

        ItemSaveDataList itemSaveDataList = JsonUtility.FromJson<ItemSaveDataList>(stringData);
        foreach (ItemSaveData saveData in itemSaveDataList.SaveDatas)
        {
            Item item = new Item(soDatas[saveData.ItemIndex], saveData);
            Add(item);
        }
    }
}

[System.Serializable]
public class ItemSaveData
{
    public ItemType ItemType;
    public int ItemIndex; // SO Index
    public int SlotIndex;

}

[Serializable]
public class ItemSaveDataList
{
   public List<ItemSaveData> SaveDatas = new List<ItemSaveData>();
}

