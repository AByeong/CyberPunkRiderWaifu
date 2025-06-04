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

    public int SlotCount = 36;

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

        if (Input.GetKeyDown(KeyCode.G))
        {
            CurrencyManager.Instance.Add(CurrencyType.Gold, 100000);
        }
    }

    public bool Add(Item item)
    {
        // _items.Add(item);
        // OnDataChanged?.Invoke();
        
        if (UI_InventoryPopup.Instance.IsInventoryFull() == false)
        {
            _items.Add(item);
            OnDataChanged?.Invoke();
            return true;
        }
        else
        {
            return false;
        }
    }
    public void Equip(Item item)
    {
        OnEquipChanged?.Invoke();
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
        string json = PlayerPrefs.GetString("Inventory", "");
        if (string.IsNullOrEmpty(json)) 
        {
            Debug.Log("저장된 인벤토리 데이터가 없습니다.");
            return;
        }

        try
        {
            ItemSaveDataList saveDataList = JsonConvert.DeserializeObject<ItemSaveDataList>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            if (saveDataList == null || saveDataList.SaveDatas == null)
            {
                Debug.LogError("저장 데이터 역직렬화 실패");
                return;
            }

            // 기존 아이템들을 먼저 정리 (필요한 경우)
            // _items.Clear();

            foreach (ItemSaveData saveData in saveDataList.SaveDatas)
            {
                // 인덱스 유효성 검사
                if (saveData.ItemSOIndex < 0 || saveData.ItemSOIndex >= soDatas.Count)
                {
                    Debug.LogError($"잘못된 ItemSOIndex: {saveData.ItemSOIndex}, soDatas 크기: {soDatas.Count}");
                    continue;
                }

                Debug.Log($"아이템 로드 중: ItemSOIndex = {saveData.ItemSOIndex}");
            
                try
                {
                    Item item = new Item(soDatas[saveData.ItemSOIndex], saveData);
                    Add(item);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"아이템 생성 실패: {e.Message}");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"인벤토리 로드 실패: {e.Message}");
            // 필요한 경우 기본값으로 초기화
            // InitializeDefaultInventory();
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

