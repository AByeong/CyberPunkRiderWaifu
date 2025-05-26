using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ItemData
{
    public string itemId;
    public int slotIndex;
}

[Serializable]
public class ItemDataList
{
    public List<ItemData> items = new List<ItemData>();
}
public class InventoryManager : Singleton<InventoryManager>
{

    public List<InventorySlot> InventorySlots { get; private set; }

    public int InventorySize => InventorySlots.Count;

    private void Awake()
    {
        // 하위에 있는 InventorySlot들을 순서대로 가져옴
        InventorySlots = new List<InventorySlot>(GetComponentsInChildren<InventorySlot>(true));
    }
    public void AddItemToInventory(Item newItem)
    {
        foreach(InventorySlot slot in InventorySlots)
        {
            if (!slot.HasItem)
            {
                slot.SetData(newItem); // 👈 아래에서 구현
                return;
            }
        }

        Debug.LogWarning("Inventory is full!");
    }
    public void SaveInventory()
    {
        List<ItemData> dataList = new List<ItemData>();

        for (int i = 0; i < InventorySlots.Count; i++)
        {
            if (InventorySlots[i].HasItem)
            {
                dataList.Add(new ItemData
                {
                    itemId = InventorySlots[i].Item.Id,
                    slotIndex = i
                });
            }
        }

        string json = JsonUtility.ToJson(new ItemDataList {items = dataList});
        PlayerPrefs.SetString("InventoryData", json);
        PlayerPrefs.Save();
    }

    public void LoadInventory()
    {
        string json = PlayerPrefs.GetString("InventoryData", "");
        if (string.IsNullOrEmpty(json)) return;

        ItemDataList dataList = JsonUtility.FromJson<ItemDataList>(json);
        foreach(ItemData data in dataList.items)
        {
            // Item item = ItemDatabase.Instance.GetItemById(data.itemId); // 👈 별도 DB 필요
            // _inventorySlots[data.slotIndex].SetItem(item);
        }
    }
}
