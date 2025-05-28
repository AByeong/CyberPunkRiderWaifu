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


    public void Start()
    {
        Load();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q");
            Item item = new Item(soDatas[1]);
            Add(item);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("C");
            Item item = new Item(soDatas[2]);
            Add(item);
        }
    }

    public void Add(Item item)
    {
        _items.Add(item);

        OnDataChanged?.Invoke();
    }

    public void AddStat(Item item)
    {
        Debug.Log($"{item.Data.ItemName} Added");
        GameManager.Instance.player.ApplyEquipment(StatType.MaxHealth,item.MaxHealth);
        GameManager.Instance.player.ApplyEquipment(StatType.AttackPower,item.AttackPower);
        GameManager.Instance.player.ApplyEquipment(StatType.Defense,item.Defense);
        GameManager.Instance.player.ApplyEquipment(StatType.Speed,item.Speed);
        GameManager.Instance.player.ApplyEquipment(StatType.AttackSpeed,item.AttackSpeed);
        GameManager.Instance.player.ApplyEquipment(StatType.CritChance,item.CritChance);
        GameManager.Instance.player.ApplyEquipment(StatType.CritDamage,item.CritDamage);
    }

    public void RemoveStat(Item item)
    {
        Debug.Log($"{item.Data.ItemName} Deleted");
        GameManager.Instance.player.RemoveEquipment(StatType.MaxHealth,item.MaxHealth);
        GameManager.Instance.player.RemoveEquipment(StatType.AttackPower,item.AttackPower);
        GameManager.Instance.player.RemoveEquipment(StatType.Defense,item.Defense);
        GameManager.Instance.player.RemoveEquipment(StatType.Speed,item.Speed);
        GameManager.Instance.player.RemoveEquipment(StatType.AttackSpeed,item.AttackSpeed);
        GameManager.Instance.player.RemoveEquipment(StatType.CritChance,item.CritChance);
        GameManager.Instance.player.RemoveEquipment(StatType.CritDamage,item.CritDamage);
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
            Item item = new Item(soDatas[saveData.ItemSOIndex], saveData);
            Add(item);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Debug.Log("나 삭제됨");
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

