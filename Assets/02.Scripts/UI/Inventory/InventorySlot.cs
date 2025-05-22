using System;
using Gamekit3D;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public enum SlotType // 인스펙터에서 설정
{
    Inventory,  // 일반 인벤토리
    Equipment   // 장비 슬롯
}
public class InventorySlot : MonoBehaviour, IDropHandler
{
    public GameObject ItemPrefab;
    public UI_Item UI_Item;
    public Item item;
    public bool HasItem;
    
    public SlotType slotType;
    public EquipmentType equipmentType; // SlotType이 Equipment일 경우 사용
    private void Start()
    {
        if (HasItem)
        {
            GameObject newitem = Instantiate(ItemPrefab, transform);
            UI_Item = newitem.GetComponent<UI_Item>();
            UI_Item.Init(item, gameObject);
        }
    }
    public void SetItem(Item newItem)
    {
        item = newItem;
        HasItem = true;

        if (UI_Item == null)
        {
            GameObject newUI = Instantiate(ItemPrefab, transform);
            UI_Item = newUI.GetComponent<UI_Item>();
        }

        UI_Item.Init(newItem, gameObject);
    }
    public void ClearSlot()
    {
        item = null;
        HasItem = false;

        if (UI_Item != null)
        {
            Destroy(UI_Item.gameObject);
            UI_Item = null;
        }
    }
     public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;
        if (!eventData.pointerDrag.TryGetComponent<UI_Item>(out var droppedUIItem)) return;

        Item droppedItem = droppedUIItem.MyItem;
        if (droppedItem == null) return;

        // 드래그된 아이템의 이전 슬롯 가져오기
        InventorySlot previousSlot = droppedUIItem.InventorySlot.GetComponent<InventorySlot>();
        // 드롭 검증
        if (!CanDropItem(droppedItem))
        {
            Debug.Log("아이템을 이 슬롯에 드롭할 수 없습니다.");
            droppedUIItem.SetPosition();
            return;
        }
        
        // 슬롯 교환 처리
        HandleSlotSwap(droppedItem, droppedUIItem, previousSlot);
        //
        // var playerController = GameManager.Instance.player;
        //
        // // 기존 슬롯에서 아이템 제거 (해제 포함)
        // if (droppedUIItem.InventorySlot.TryGetComponent(out InventorySlot prevSlot))
        // {
        //     if (prevSlot.slotType == SlotType.Equipment && droppedItem.ItemType == ItemType.Equipment)
        //     {
        //         // 이전 슬롯이 장비 슬롯이면 장비 해제
        //         if (droppedItem.EquipmentData != null)
        //         {
        //             foreach (var stat in droppedItem.EquipmentData.Stats)
        //             {
        //                 playerController.RemoveEquipment(stat.Key, stat.Value);
        //                 Debug.Log($"[장비 해제] {stat.Key} : {stat.Value}");
        //             }
        //         }
        //
        //         prevSlot.ClearSlot();
        //     }
        // }
        //
        // // 장비 아이템인 경우
        // if (droppedItem.ItemType == ItemType.Equipment)
        // {
        //     if (slotType != SlotType.Equipment)
        //     {
        //         Debug.Log("장비가 아닌 슬롯에 드롭됨 -> 원래 위치로 복귀");
        //         droppedUIItem.SetPosition();
        //         return;
        //     }
        //
        //     if (droppedItem.EquipmentData == null || droppedItem.EquipmentData.EquipmentType != equipmentType)
        //     {
        //         Debug.Log("장비 타입 불일치 -> 원래 위치로 복귀");
        //         droppedUIItem.SetPosition();
        //         return;
        //     }
        //
        //     // 현재 장비 제거
        //     if (item != null && item.ItemType == ItemType.Equipment && item.EquipmentData != null)
        //     {
        //         foreach (var stat in item.EquipmentData.Stats)
        //         {
        //             playerController.RemoveEquipment(stat.Key, stat.Value);
        //             Debug.Log($"[장비 교체] {stat.Key} : {stat.Value}");
        //         }
        //     }
        //     item = droppedItem;
        //     // 장착
        //     foreach (var stat in droppedItem.EquipmentData.Stats)
        //     {
        //         playerController.ApplyEquipment(stat.Key, stat.Value);
        //         Debug.Log($"[장착 완료] {stat.Key} : {stat.Value}");
        //     }
        // }
        // else
        // {
        //     // 일반 아이템은 Inventory 슬롯에만 들어갈 수 있음
        //     if (slotType != SlotType.Inventory)
        //     {
        //         Debug.Log("소비/기타 아이템은 장비 슬롯에 들어갈 수 없음");
        //         droppedUIItem.SetPosition();
        //         return;
        //     }
        //     item = droppedItem;
        // }
        // droppedUIItem.RemoveSlotItem();
        // droppedUIItem.SetItem(gameObject);
        // droppedUIItem.SetPosition();
    }
    private bool CanDropItem(Item droppedItem)
    {
        // 장비 아이템인 경우
        if (droppedItem.ItemType == ItemType.Equipment)
        {
            // 장비 슬롯이 아니면 드롭 불가
            if (slotType != SlotType.Equipment)
                return false;
                
            // 장비 타입이 맞지 않으면 드롭 불가
            if (droppedItem.EquipmentData == null || droppedItem.EquipmentData.EquipmentType != equipmentType)
                return false;
        }
        else
        {
            // 일반 아이템은 인벤토리 슬롯에만 드롭 가능
            if (slotType != SlotType.Inventory)
                return false;
        }
        
        return true;
    }
      private void HandleSlotSwap(Item droppedItem, UI_Item droppedUIItem, InventorySlot previousSlot)
    {
        PlayerController playerController = GameManager.Instance.player.GetComponent<PlayerController>();
        
        // 현재 슬롯에 아이템이 있는 경우 (스왑)
        if (HasItem && item != null)
        {
            Item currentItem = item;
            UI_Item currentUIItem = UI_Item;
            
            // 현재 아이템이 이전 슬롯에 들어갈 수 있는지 확인
            if (!CanItemGoToSlot(currentItem, previousSlot))
            {
                Debug.Log("아이템 교환이 불가능합니다.");
                droppedUIItem.SetPosition();
                return;
            }
            
            // 현재 슬롯의 장비 해제 (장비인 경우)
            if (slotType == SlotType.Equipment && currentItem.ItemType == ItemType.Equipment)
            {
                UnequipItem(currentItem);
            }
            
            // 이전 슬롯의 장비 해제 (장비인 경우)
            if (previousSlot.slotType == SlotType.Equipment && droppedItem.ItemType == ItemType.Equipment)
            {
                UnequipItem(droppedItem);
            }
            
            // 현재 슬롯 비우기
            ClearSlot();
            
            // 드롭된 아이템을 현재 슬롯에 설정
            SetItem(droppedItem);
            droppedUIItem.SetItem(gameObject);
            
            // 현재 아이템을 이전 슬롯에 설정
            previousSlot.SetItem(currentItem);
            currentUIItem.SetItem(previousSlot.gameObject);
            
            // 장비 아이템인 경우 장착 처리
            if (slotType == SlotType.Equipment && droppedItem.ItemType == ItemType.Equipment)
            {
                EquipItem(droppedItem);
            }
            
            if (previousSlot.slotType == SlotType.Equipment && currentItem.ItemType == ItemType.Equipment)
            {
                EquipItem(currentItem);
            }
        }
        else
        {
            // 빈 슬롯에 드롭하는 경우
            // 이전 슬롯의 장비 해제 (장비인 경우)
            if (previousSlot.slotType == SlotType.Equipment && droppedItem.ItemType == ItemType.Equipment)
            {
                UnequipItem(droppedItem);
            }
            
            // 이전 슬롯 비우기
            previousSlot.ClearSlot();
            
            // 현재 슬롯에 아이템 설정
            SetItem(droppedItem);
            droppedUIItem.SetItem(gameObject);
            
            // 장비 아이템인 경우 장착 처리
            if (slotType == SlotType.Equipment && droppedItem.ItemType == ItemType.Equipment)
            {
                EquipItem(droppedItem);
            }
        }
        
        droppedUIItem.SetPosition();
    }
    private bool CanItemGoToSlot(Item item, InventorySlot targetSlot)
    {
        if (item.ItemType == ItemType.Equipment)
        {
            if (targetSlot.slotType != SlotType.Equipment)
                return false;
                
            if (item.EquipmentData == null || item.EquipmentData.EquipmentType != targetSlot.equipmentType)
                return false;
        }
        else
        {
            if (targetSlot.slotType != SlotType.Inventory)
                return false;
        }
        
        return true;
    }
    private void UnequipItem(Item equipItem )
    {
        PlayerController playerController = GameManager.Instance.player.GetComponent<PlayerController>();
        if (equipItem.EquipmentData != null)
        {
            foreach (var stat in equipItem.EquipmentData.Stats)
            {
                playerController.RemoveEquipment(stat.Key, stat.Value);
                Debug.Log($"[장비 해제] {stat.Key} : {stat.Value}");
            }
        }
    }
    private void EquipItem(Item equipItem)
    {
        if (equipItem.EquipmentData != null)
        {
            PlayerController playerController = GameManager.Instance.player.GetComponent<PlayerController>();
            foreach (var stat in equipItem.EquipmentData.Stats)
            {
                playerController.ApplyEquipment(stat.Key, stat.Value);
                Debug.Log($"[장착 완료] {stat.Key} : {stat.Value}");
            }
        }
        // PlayerController playerController = GameManager.Instance.player;
        //
        // // 기존 장비 제거
        // if (item != null && item.ItemType == ItemType.Equipment && item.EquipmentData != null)
        // {
        //     foreach (var stat in item.EquipmentData.Stats)
        //     {
        //         playerController.RemoveEquipment(stat.Key, stat.Value);
        //         Debug.Log($"[장비 해제] {stat.Key} : {stat.Value}");
        //     }
        // }
        //
        // item = newItem;
        // HasItem = true;
        //
        // droppedUIItem.RemoveSlotItem();
        // droppedUIItem.SetItem(gameObject);
        // droppedUIItem.SetPosition();
        //
        // if (newItem.EquipmentData != null)
        // {
        //     foreach (var stat in newItem.EquipmentData.Stats)
        //     {
        //         playerController.ApplyEquipment(stat.Key, stat.Value);
        //         Debug.Log($"[장착 완료] {stat.Key} : {stat.Value}");
        //     }
        // }
    }
}
