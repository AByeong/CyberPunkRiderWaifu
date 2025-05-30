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

    public int SlotCount = 36;
    public bool IsInventoryFull => _items.Count == SlotCount;

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

    public bool Add(Item item)
    {
        if (UI_InventoryPopup.Instance.IsInventoryFull() == false)
        {
            _items.Add(item);
            OnDataChanged?.Invoke();
            return true;
        }
        else
        {
            Debug.Log("인벤토리 꽉참ㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋ");
            return false;
        }
    }

    public void Remove(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("삭제하려는 아이템이 null입니다.");
            return;
        }
    
        bool removed = _items.Remove(item);
    
        if (removed)
        {
            Debug.Log($"{item.Data.ItemName} 아이템이 인벤토리에서 제거되었습니다.");
            OnDataChanged?.Invoke();
        }
        else
        {
            Debug.LogWarning($"{item.Data.ItemName} 아이템을 인벤토리에서 찾을 수 없습니다.");
        }
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
        Debug.Log($"playerStat 참조 안됨, ItemSpeed : {item.Speed}"); // 플레이어 Stat 참조 안됨
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

