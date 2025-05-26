using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ItemData
{
    public string itemId;
    public int slotIndex;
}

[System.Serializable]
public class ItemDataList
{
    public List<ItemData> items = new List<ItemData>();
}
public class InventoryManager : Singleton<InventoryManager>
{
    private List<InventorySlot> _inventorySlots;

    public List<InventorySlot> InventorySlots =>  _inventorySlots;
    
    public int InventorySize => InventorySlots.Count;
    
    private void Awake()
    {
        // 하위에 있는 InventorySlot들을 순서대로 가져옴
        _inventorySlots = new List<InventorySlot>(GetComponentsInChildren<InventorySlot>(true));
    }
    public void AddItemToInventory(Item newItem)
    {
        foreach (var slot in _inventorySlots)
        {
            if (!slot.HasItem)
            {
                slot.SetItem(newItem); // 👈 아래에서 구현
                return;
            }
        }

        Debug.LogWarning("Inventory is full!");
    }
    public void SaveInventory()
    {
        List<ItemData> dataList = new List<ItemData>();

        for (int i = 0; i < _inventorySlots.Count; i++)
        {
            if (_inventorySlots[i].HasItem)
            {
                dataList.Add(new ItemData
                {
                    itemId = _inventorySlots[i].Item.Id,
                    slotIndex = i
                });
            }
        }

        string json = JsonUtility.ToJson(new ItemDataList { items = dataList });
        PlayerPrefs.SetString("InventoryData", json);
        PlayerPrefs.Save();
    }

    public void LoadInventory()
    {
        string json = PlayerPrefs.GetString("InventoryData", "");
        if (string.IsNullOrEmpty(json)) return;

        ItemDataList dataList = JsonUtility.FromJson<ItemDataList>(json);
        foreach (var data in dataList.items)
        {
            // Item item = ItemDatabase.Instance.GetItemById(data.itemId); // 👈 별도 DB 필요
            // _inventorySlots[data.slotIndex].SetItem(item);
        }
    }
}
