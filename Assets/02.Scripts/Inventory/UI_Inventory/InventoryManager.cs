using System;
using System.Collections.Generic;
using JY;
using UnityEngine;
using UnityEngine.Serialization;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

[System.Serializable]
public class InventoryManager : Singleton<InventoryManager>
{
    public List<ItemBaseDataSO> soDatas;

    private List<Item> _items = new List<Item>();
    public List<Item> Items => _items;

    public Action OnDataChanged;
    public Action OnEquipChanged;
    public List<Item> GetEquippedItems()
    {
        return _items.FindAll(item => item.IsEquipped);
    }


    public void Start()
    {
        Load();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q");
            Item item = ItemCreateManager.Instance.CreateHead();
            Add(item);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("C");
            Item item =ItemCreateManager.Instance.CreateBoots();
            Add(item);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            var dropPlan = new Dictionary<DropItemType, int>
            {
                { DropItemType.Item, 3 },
            };
            ItemDropManager.Instance.DropItems(dropPlan, transform.position, transform.forward); 
        }
    }

    public void Add(Item item)
    {
        _items.Add(item);
        OnDataChanged?.Invoke();
    }

    public void Equip(Item item)
    {
        
        OnEquipChanged?.Invoke();
    }
    
    private void Save()
    {
        ItemSaveDataList dataList = new ItemSaveDataList();
        foreach (var item in _items)
        {
            ItemSaveData saveData = item.ToSaveData(soDatas.IndexOf(item.Data));
            dataList.SaveDatas.Add(saveData);
        }
        string json = JsonConvert.SerializeObject(dataList, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        PlayerPrefs.SetString("Inventory", json);
    }

    private void Load()
    {
        string json =  PlayerPrefs.GetString("Inventory", "");
        if (string.IsNullOrEmpty(json)) return;
        ItemSaveDataList saveDataList = JsonConvert.DeserializeObject<ItemSaveDataList>(json, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });

        foreach (ItemSaveData saveData in saveDataList.SaveDatas)
        {
            Debug.Log(saveData.ItemSOIndex);
            Item item = new Item(soDatas[saveData.ItemSOIndex], saveData);
            Add(item);
        }
    }
}

[System.Serializable]
public class ItemSaveData
{
    public ItemType ItemType;
    public SlotType SlotType;
    public int ItemSOIndex; // SO Index
    public int SlotIndex;
}
[System.Serializable]
public class EquipmentSaveData : ItemSaveData
{
    public float MaxHealth;
    public float AttackPower;
    public float Defense;
    public float Speed;
    public float AttackSpeed;
    public float CritChance;
    public float CritDamage;
    public SetItemType SetItemType;
}

[System.Serializable]
public class ChipSaveData : ItemSaveData
{
    public float SkillRange;
    public float ReduceCooldown;
}
[Serializable]
public class ItemSaveDataList
{
   public List<ItemSaveData> SaveDatas = new List<ItemSaveData>();
}

