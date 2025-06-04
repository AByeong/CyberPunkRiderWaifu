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
        Load(); // 게임 시작 시 로드
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            CurrencyManager.Instance.Add(CurrencyType.Gold, 100000);
        }
        
        // 저장 단축키 (테스트용)
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Save();
            Debug.Log("인벤토리가 저장되었습니다.");
        }
        
        // 로드 단축키 (테스트용)
        if (Input.GetKeyDown(KeyCode.F9))
        {
            Load();
            Debug.Log("인벤토리가 로드되었습니다.");
        }
    }

    public bool Add(Item item)
    {
        if (UI_InventoryPopup.Instance.IsInventoryFull() == false)
        {
            _items.Add(item);
            OnDataChanged?.Invoke();
            Save(); // 자동 저장
            return true;
        }
        else
        {
            Debug.Log("가방이 꽉참");
            return false;
        }
    }
    
    public void Equip(Item item)
    {
        OnEquipChanged?.Invoke();
        Save(); // 자동 저장
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
            Save(); // 자동 저장
        }
        else
        {
            Debug.LogWarning($"{item.Data.ItemName} 아이템을 인벤토리에서 찾을 수 없습니다.");
        }
    }

    private void Save()
    {
        try
        {
            string json = JsonConvert.SerializeObject(_items, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            
            PlayerPrefs.SetString("Inventory", json);
            PlayerPrefs.Save();
            
            Debug.Log($"인벤토리 저장 완료: {_items.Count}개 아이템");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"인벤토리 저장 실패: {e.Message}");
        }
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
            // 먼저 새로운 형식(직접 List<Item>)으로 로드 시도
            if (json.StartsWith("["))
            {
                // 새로운 형식: 직접 List<Item>
                List<Item> loadedItems = JsonConvert.DeserializeObject<List<Item>>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                if (loadedItems != null)
                {
                    _items = loadedItems;
                    OnDataChanged?.Invoke();
                    Debug.Log($"인벤토리 로드 완료 (새 형식): {_items.Count}개 아이템");
                }
            }
            else
            {
                // 기존 형식: ItemSaveDataList 형태
                Debug.Log("기존 저장 형식 감지됨. 변환 중...");
                
                // 기존 데이터 삭제하고 새로 시작
                PlayerPrefs.DeleteKey("Inventory");
                InitializeDefaultInventory();
                
                Debug.Log("기존 데이터를 삭제했습니다. 새로운 형식으로 저장됩니다.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"인벤토리 로드 실패: {e.Message}");
            Debug.Log("저장 데이터를 초기화합니다.");
            
            // 문제가 있는 데이터 삭제
            PlayerPrefs.DeleteKey("Inventory");
            InitializeDefaultInventory();
        }
    }
    
    private void InitializeDefaultInventory()
    {
        _items.Clear();
        Debug.Log("기본 인벤토리로 초기화되었습니다.");
        OnDataChanged?.Invoke();
    }
    
    // 수동 저장/로드 메서드 (UI에서 호출 가능)
    public void SaveInventory() => Save();
    public void LoadInventory() => Load();
    
    public void ClearInventory()
    {
        _items.Clear();
        OnDataChanged?.Invoke();
        Save();
        Debug.Log("인벤토리가 초기화되었습니다.");
    }
}

// 기존 SaveData 클래스들은 이제 필요 없음 (Item 클래스가 직접 직렬화됨)
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