using System.Collections.Generic;
using Gamekit3D;
using UnityEngine;
using UnityEngine.EventSystems;
public enum SlotType // 인스펙터에서 설정
{
    Inventory, // 일반 인벤토리
    Equipment // 장비 슬롯
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
            SetItem(UI_Item.MyItem);
            UI_Item.Init(item, gameObject);
        }
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            Debug.Log("OnDrop 중단: pointerDrag가 null입니다.");
            return;
        }

        if (!eventData.pointerDrag.TryGetComponent(out UI_Item droppedUIItem))
        {
            Debug.Log("OnDrop 중단: pointerDrag에서 UI_Item 컴포넌트를 가져오지 못했습니다.");
            return;
        }

        Item droppedItem = droppedUIItem.MyItem;
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

        Debug.Log($"드롭 시도: {droppedItem.ItemName} ({droppedItem.ItemType}) -> {slotType} 슬롯");

        // 드롭 가능 여부 검증
        if (!CanDropItemToThisSlot(droppedItem))
        {
            Debug.Log($"드롭 불가: {droppedItem.ItemName}을(를) {slotType} 슬롯에 드롭할 수 없습니다.");
            droppedUIItem.SetPosition();
            return;
        }

        // 아이템 이동/교환 처리
        Debug.Log("드롭 처리: 아이템 이동 또는 교환을 진행합니다.");
        ProcessItemDrop(droppedItem, droppedUIItem, previousSlot);
    }

    public virtual void SetItem(Item newItem)
    {
        item = newItem;
        HasItem = true;

        if (UI_Item == null)
        {
            GameObject newUI = Instantiate(ItemPrefab, transform);
            UI_Item = newUI.GetComponent<UI_Item>();
            item = newItem;
        }

        UI_Item.Init(newItem, gameObject);
    }

    public void ClearSlot()
    {
        item = null;
        HasItem = false;

    }

    private bool CanDropItemToThisSlot(Item droppedItem)
    {
        // 인벤토리 슬롯은 모든 아이템 수용 가능
        if (slotType == SlotType.Inventory)
        {
            return true;
        }

        // 장비 슬롯은 장비 아이템만 수용 가능
        if (slotType == SlotType.Equipment)
        {
            // 장비 아이템이 아니면 드롭 불가
            if (droppedItem.ItemType != ItemType.Equipment)
            {
                Debug.Log("장비 슬롯에는 장비 아이템만 드롭 가능합니다.");
                return false;
            }

            // 장비 타입이 맞지 않으면 드롭 불가
            if (droppedItem.EquipmentData == null || droppedItem.EquipmentData.EquipmentType != equipmentType)
            {
                Debug.Log($"장비 타입 불일치: 필요 타입 {equipmentType}, 아이템 타입 {droppedItem.EquipmentData?.EquipmentType}");
                return false;
            }
        }

        return true;
    }

    private void ProcessItemDrop(Item droppedItem, UI_Item droppedUIItem, InventorySlot previousSlot)
    {
        PlayerController playerController = GameManager.Instance.player;

        // 현재 슬롯에 아이템이 있는 경우 (교환)
        if (HasItem && item != null)
        {
            Debug.Log($"아이템 교환 시도: {item.ItemName} <-> {droppedItem.ItemName}");

            // 현재 슬롯의 아이템이 이전 슬롯에 들어갈 수 있는지 확인
            if (!CanItemGoToSlot(item, previousSlot))
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
        if (targetSlot.slotType == SlotType.Inventory)
        {
            return true;
        }

        // 장비 슬롯 검증
        if (targetSlot.slotType == SlotType.Equipment)
        {
            if (targetItem.ItemType != ItemType.Equipment)
                return false;

            if (targetItem.EquipmentData == null || targetItem.EquipmentData.EquipmentType != targetSlot.equipmentType)
                return false;
        }

        return true;
    }

    private void SwapItems(Item droppedItem, UI_Item droppedUIItem, InventorySlot previousSlot, PlayerController playerController)
    {
        // 현재 슬롯의 아이템과 UI 임시 저장
        Item currentItem = item;
        UI_Item currentUIItem = UI_Item;

        Debug.Log($"아이템 교환 실행: {currentItem.ItemName} <-> {droppedItem.ItemName}");

        // 1. 장비 해제 처리 (이동 전)
        UnequipIfNeeded(droppedItem, previousSlot, playerController);
        UnequipIfNeeded(currentItem, this, playerController);

        // 2. UI 연결 해제
        droppedUIItem.RemoveSlotItem();

        // 3. 슬롯 데이터 초기화
        item = null;
        HasItem = false;
        UI_Item = null;

        previousSlot.item = null;
        previousSlot.HasItem = false;
        previousSlot.UI_Item = null;

        // 4. 아이템 재배치
        // 드롭된 아이템을 현재 슬롯에
        item = droppedItem;
        HasItem = true;
        UI_Item = droppedUIItem;
        droppedUIItem.SetItem(gameObject);
        droppedUIItem.SetPosition();
        SetItem(UI_Item.MyItem);

        // 현재 아이템을 이전 슬롯에
        previousSlot.item = currentItem;
        previousSlot.HasItem = true;
        previousSlot.UI_Item = currentUIItem;
        currentUIItem.SetItem(previousSlot.gameObject);
        currentUIItem.SetPosition();

        // 5. 장비 장착 처리 (이동 후)
        EquipIfNeeded(droppedItem, this, playerController);
        EquipIfNeeded(currentItem, previousSlot, playerController);

        Debug.Log("아이템 교환 완료");
    }

    private void MoveItemToEmptySlot(Item droppedItem, UI_Item droppedUIItem, InventorySlot previousSlot, PlayerController playerController)
    {
        Debug.Log($"아이템 이동 실행: {droppedItem.ItemName} -> {slotType} 슬롯");

        // 1. 장비 해제 처리 (이전 슬롯이 장비 슬롯인 경우)
        UnequipIfNeeded(droppedItem, previousSlot, playerController);

        // 2. UI 연결 해제
        droppedUIItem.RemoveSlotItem();

        // 3. 이전 슬롯 초기화
        previousSlot.item = null;
        previousSlot.HasItem = false;
        previousSlot.UI_Item = null;

        // 4. 현재 슬롯에 아이템 설정
        item = droppedItem;
        HasItem = true;
        UI_Item = droppedUIItem;
        droppedUIItem.SetItem(gameObject);
        droppedUIItem.SetPosition();
        SetItem(item);
        // 5. 장비 장착 처리 (현재 슬롯이 장비 슬롯인 경우)
        EquipIfNeeded(droppedItem, this, playerController);

        Debug.Log("아이템 이동 완료");
    }

    private void EquipIfNeeded(Item equipItem, InventorySlot slot, PlayerController playerController)
    {
        if (slot.slotType == SlotType.Equipment &&
            equipItem.ItemType == ItemType.Equipment &&
            equipItem.EquipmentData != null)
        {
            foreach(KeyValuePair<StatType, float> stat in equipItem.EquipmentData.Stats)
            {
                playerController.ApplyEquipment(stat.Key, stat.Value);
                Debug.Log($"[장착 완료] {equipItem.ItemName} - {stat.Key} : +{stat.Value}");
            }
        }
    }

    private void UnequipIfNeeded(Item equipItem, InventorySlot slot, PlayerController playerController)
    {
        if (slot.slotType == SlotType.Equipment &&
            equipItem.ItemType == ItemType.Equipment &&
            equipItem.EquipmentData != null)
        {
            foreach(KeyValuePair<StatType, float> stat in equipItem.EquipmentData.Stats)
            {
                playerController.RemoveEquipment(stat.Key, stat.Value);
                Debug.Log($"[장비 해제] {equipItem.ItemName} - {stat.Key} : -{stat.Value}");
            }
        }
    }
}
