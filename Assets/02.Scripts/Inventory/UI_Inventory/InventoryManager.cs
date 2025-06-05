using System;
using System.Collections.Generic;
using JY;
using UnityEngine;
using UnityEngine.Serialization;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
[System.Serializable]
public class StringListWrapper
{
    public List<string> items;
}
[System.Serializable]
public class InventoryManager : Singleton<InventoryManager>
{
    public List<Item> _items = new List<Item>();
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
            Debug.Log("정상적으로 Item Add");
            _items.Add(item);
            OnDataChanged?.Invoke();
            Save(); // 자동 저장
            return true;
        }
        Debug.Log("가방이 꽉참");
        return false;
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
            Save(); // 자동 저장
        }
        else
        {
            Debug.LogWarning($"{item.Data.ItemName} 아이템을 인벤토리에서 찾을 수 없습니다.");
        }
    }

    private void Save()
    {
    }

    private void Load()
    {
        
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
        Debug.Log("인벤토리가 초기화되었습니다.");
    }

    private ItemBaseDataSO FindSOById(string id)
    {
        // Resources 폴더에서 모든 SO를 불러와서 id로 찾기
        var allSOs = Resources.LoadAll<ItemBaseDataSO>("SO경로");
        foreach (var so in allSOs)
        {
            if (so.Id == id) return so;
        }
        return null;
    }
}

// 기존 SaveData 클래스들은 이제 필요 없음 (Item 클래스가 직접 직렬화됨)
[System.Serializable]
public class ItemSaveData
{
    public string Id;
    public int SOIndex;
    public ItemType ItemType;
    public SlotType SlotType;
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
    public List<CubeStat> CubeStats;
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