using System.Collections.Generic;
using JY;
using UnityEngine;
using UnityEngine.EventSystems;
public enum SlotType // 인스펙터에서 설정
{
    Inventory, // 일반 인벤토리
    Equipment // 장비 슬롯
}

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public SlotType SlotType;
    // public UI_Item UI_Item;
    public UI_Itembase[] Items;

    public Item Item;
    // public Item item;
    public bool HasItem;

    public EquipmentType equipmentType; // SlotType이 Equipment일 경우 사용

    private void Start()
    {
        if (HasItem)
        {
            switch(SlotType)
            {
                case SlotType.Equipment:
                {
                    Items[0].gameObject.SetActive(true);
                    Items[0].Init(Item, this.gameObject);
                    break;
                }
            }
        }
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            Debug.Log("OnDrop 중단: pointerDrag가 null입니다.");
            return;
        }

        if (!eventData.pointerDrag.TryGetComponent(out UI_Itembase droppedUIItem))
        {
            Debug.Log("OnDrop 중단: pointerDrag에서 UI_Item 컴포넌트를 가져오지 못했습니다.");
            return;
        }

        Item droppedItem = droppedUIItem.Item;
        if (droppedItem == null)
        {
            Debug.Log("OnDrop 중단: 드롭된 아이템이 null입니다.");
            return;
        }

        // 드래그된 아이템의 이전 슬롯 가져오기
        GameObject previousSlotObject = droppedUIItem.InventorySlot;
        if (previousSlotObject == null)
        {
            Debug.Log("OnDrop 중단: 드래그된 아이템의 이전 슬롯 오브젝트가 null입니다.");
            return;
        }

        InventorySlot previousSlot = previousSlotObject.GetComponent<InventorySlot>();
        if (previousSlot == null)
        {
            Debug.Log("OnDrop 중단: 이전 슬롯에서 InventorySlot 컴포넌트를 가져오지 못했습니다.");
            return;
        }

        // 같은 슬롯에 드롭한 경우 무시
        if (previousSlot == this)
        {
            Debug.Log("OnDrop 중단: 같은 슬롯에 드롭되었습니다.");
            droppedUIItem.SetPosition();
            return;
        }


        // 드롭 가능 여부 검증
        if (!CanDropItemToThisSlot(droppedItem))
        {
            Debug.Log($"드롭 불가: {droppedItem.ItemName}을(를) {SlotType} 슬롯에 드롭할 수 없습니다.");
            droppedUIItem.SetPosition();
            return;
        }

        // 아이템 이동/교환 처리
        Debug.Log("드롭 처리: 아이템 이동 또는 교환을 진행합니다.");
        ProcessItemDrop(droppedItem, droppedUIItem, previousSlot);
    }

    public virtual void SetItem(Item newItem)
    {
        // item = newItem;
        // HasItem = true;
        //
        // if (UI_Item == null)
        // {
        //     GameObject newUI = Instantiate(ItemPrefab, transform);
        //     UI_Item = newUI.GetComponent<UI_Item>();
        //     item = newItem;
        // }
        //
        // UI_Item.Init(item, gameObject);
    }

    public virtual void ClearSlot()
    {
        // item = null;
        // HasItem = false;
        // UI_Item = null;
    }

    private bool CanDropItemToThisSlot(Item droppedItem)
    {
        // 인벤토리 슬롯은 모든 아이템 수용 가능
        if (SlotType == SlotType.Inventory)
        {
            return true;
        }

        // 장비 슬롯은 장비 아이템만 수용 가능
        if (SlotType == SlotType.Equipment)
        {
            // 장비 아이템이 아니면 드롭 불가
            // if (droppedItem.SlotType != ItemType.Equipment)
            // {
            //     Debug.Log("장비 슬롯에는 장비 아이템만 드롭 가능합니다.");
            //     return false;
            // }
            //
            // // 장비 타입이 맞지 않으면 드롭 불가
            // if (droppedItem.EquipmentData == null || droppedItem.EquipmentData.EquipmentType != equipmentType)
            // {
            //     Debug.Log($"장비 타입 불일치: 필요 타입 {equipmentType}, 아이템 타입 {droppedItem.EquipmentData?.EquipmentType}");
            //     return false;
            // }
        }

        return true;
    }

    private void ProcessItemDrop(Item droppedItem, UI_Itembase droppedUIItem, InventorySlot previousSlot)
    {
        PlayerController playerController = GameManager.Instance.player;

        // 현재 슬롯에 아이템이 있는 경우 (교환)
        if (HasItem && Item != null)
        {
            // 현재 슬롯의 아이템이 이전 슬롯에 들어갈 수 있는지 확인
            if (!CanItemGoToSlot(Item, previousSlot))
            {
                Debug.Log("아이템 교환 불가: 현재 아이템이 이전 슬롯에 들어갈 수 없습니다.");
                droppedUIItem.SetPosition();
                return;
            }

            // 아이템 교환 처리
            SwapItems(droppedItem, droppedUIItem, previousSlot, playerController);
        }
        else
        {
            Debug.Log($"빈 슬롯으로 아이템 이동: {droppedItem.ItemName}");
            // 빈 슬롯에 아이템 이동
            MoveItemToEmptySlot(droppedItem, droppedUIItem, previousSlot, playerController);
        }
    }

    private bool CanItemGoToSlot(Item targetItem, InventorySlot targetSlot)
    {
        // 인벤토리 슬롯은 모든 아이템 수용 가능
        if (targetSlot.SlotType == SlotType.Inventory)
        {
            return true;
        }

        // 장비 슬롯 검증
        if (targetSlot.SlotType == SlotType.Equipment)
        {
            // if (targetItem.ItemType != ItemType.Equipment)
            //     return false;
            //
            // if (targetItem.EquipmentData == null || targetItem.EquipmentData.EquipmentType != targetSlot.equipmentType)
            //     return false;
        }

        return true;
    }

    private void SwapItems(Item droppedItem, UI_Itembase droppedUIItem, InventorySlot previousSlot, PlayerController playerController)
    {
       
    }

    private void MoveItemToEmptySlot(Item droppedItem, UI_Itembase droppedUIItem, InventorySlot previousSlot, PlayerController playerController)
    {
        
    }

    private void EquipIfNeeded(Item equipItem, InventorySlot slot, PlayerController playerController)
    {
        // if (slot.SlotType == SlotType.Equipment &&
        //     equipItem.ItemType == ItemType.Equipment &&
        //     equipItem.EquipmentData != null)
        // {
        //     foreach(KeyValuePair<StatType, float> stat in equipItem.EquipmentData.Stats)
        //     {
        //         playerController.ApplyEquipment(stat.Key, stat.Value);
        //         Debug.Log($"[장착 완료] {equipItem.ItemName} - {stat.Key} : +{stat.Value}");
        //     }
        // }
    }

    private void UnequipIfNeeded(Item equipItem, InventorySlot slot, PlayerController playerController)
    {
    //     if (slot.SlotType == SlotType.Equipment &&
    //         equipItem.ItemType == ItemType.Equipment &&
    //         equipItem.EquipmentData != null)
    //     {
    //         foreach(KeyValuePair<StatType, float> stat in equipItem.EquipmentData.Stats)
    //         {
    //             playerController.RemoveEquipment(stat.Key, stat.Value);
    //             Debug.Log($"[장비 해제] {equipItem.ItemName} - {stat.Key} : -{stat.Value}");
    //         }
    //     }
    // }
    }
}
