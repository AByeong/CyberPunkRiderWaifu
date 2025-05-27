using System.Collections.Generic;
using JY;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public enum SlotType // 인스펙터에서 설정
{
    Inventory, // 일반 인벤토리
    Equipment // 장비 슬롯
}

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public GameObject ItemPrefab;
    public UI_Item UI_Item;
    [FormerlySerializedAs("item")] public ItemData itemData;
    public bool HasItem;

    public SlotType slotType;
    public EquipmentType equipmentType; // SlotType이 Equipment일 경우 사용

    private void Start()
    {
        if (HasItem)
        {
            GameObject newitem = Instantiate(ItemPrefab, transform);

            UI_Item = newitem.GetComponent<UI_Item>();
            SetItem(UI_Item.myItemData);
            UI_Item.Init(itemData, gameObject);
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

        ItemData droppedItemData = droppedUIItem.myItemData;
        if (droppedItemData == null)
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

        Debug.Log($"드롭 시도: {droppedItemData.ItemName} ({droppedItemData.ItemType}) -> {slotType} 슬롯");

        // 드롭 가능 여부 검증
        if (!CanDropItemToThisSlot(droppedItemData))
        {
            Debug.Log($"드롭 불가: {droppedItemData.ItemName}을(를) {slotType} 슬롯에 드롭할 수 없습니다.");
            droppedUIItem.SetPosition();
            return;
        }

        // 아이템 이동/교환 처리
        Debug.Log("드롭 처리: 아이템 이동 또는 교환을 진행합니다.");
        ProcessItemDrop(droppedItemData, droppedUIItem, previousSlot);
    }

    public virtual void SetItem(ItemData newItemData)
    {
        itemData = newItemData;
        HasItem = true;

        if (UI_Item == null)
        {
            GameObject newUI = Instantiate(ItemPrefab, transform);
            UI_Item = newUI.GetComponent<UI_Item>();
            itemData = newItemData;
        }

        UI_Item.Init(itemData, gameObject);
    }

    public virtual void ClearSlot()
    {
        itemData = null;
        HasItem = false;
        UI_Item = null;
    }

    private bool CanDropItemToThisSlot(ItemData droppedItemData)
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
            if (droppedItemData.ItemType != ItemType.Equipment)
            {
                Debug.Log("장비 슬롯에는 장비 아이템만 드롭 가능합니다.");
                return false;
            }

            // 장비 타입이 맞지 않으면 드롭 불가
            if (droppedItemData.Data == null || ((EquipmentData)droppedItemData.Data).EquipmentType != equipmentType)
            {
                Debug.Log($"장비 타입 불일치: 필요 타입 {equipmentType}, 아이템 타입 {((EquipmentData)droppedItemData.Data)?.EquipmentType}");
                return false;
            }
        }

        return true;
    }

    private void ProcessItemDrop(ItemData droppedItemData, UI_Item droppedUIItem, InventorySlot previousSlot)
    {
        PlayerController playerController = GameManager.Instance.player;

        // 현재 슬롯에 아이템이 있는 경우 (교환)
        if (HasItem && itemData != null)
        {
            Debug.Log($"아이템 교환 시도: {itemData.ItemName} <-> {droppedItemData.ItemName}");

            // 현재 슬롯의 아이템이 이전 슬롯에 들어갈 수 있는지 확인
            if (!CanItemGoToSlot(itemData, previousSlot))
            {
                Debug.Log("아이템 교환 불가: 현재 아이템이 이전 슬롯에 들어갈 수 없습니다.");
                droppedUIItem.SetPosition();
                return;
            }

            // 아이템 교환 처리
            SwapItems(droppedItemData, droppedUIItem, previousSlot, playerController);
        }
        else
        {
            Debug.Log($"빈 슬롯으로 아이템 이동: {droppedItemData.ItemName}");
            // 빈 슬롯에 아이템 이동
            MoveItemToEmptySlot(droppedItemData, droppedUIItem, previousSlot, playerController);
        }
    }

    private bool CanItemGoToSlot(ItemData targetItemData, InventorySlot targetSlot)
    {
        // 인벤토리 슬롯은 모든 아이템 수용 가능
        if (targetSlot.slotType == SlotType.Inventory)
        {
            return true;
        }

        // 장비 슬롯 검증
        if (targetSlot.slotType == SlotType.Equipment)
        {
            
            if (targetItemData.ItemType != ItemType.Equipment)
                return false;

            if (((EquipmentData)targetItemData.Data) == null || ((EquipmentData)targetItemData.Data).EquipmentType != targetSlot.equipmentType)
                return false;
        }

        return true;
    }

    private void SwapItems(ItemData droppedItemData, UI_Item droppedUIItem, InventorySlot previousSlot, PlayerController playerController)
    {
        // 현재 슬롯의 아이템과 UI 임시 저장
        ItemData currentItemData = itemData;
        UI_Item currentUIItem = UI_Item;

        Debug.Log($"아이템 교환 실행: {currentItemData.ItemName} <-> {droppedItemData.ItemName}");

        // 1. 장비 해제 처리 (이동 전)
        UnequipIfNeeded(droppedItemData, previousSlot, playerController);
        UnequipIfNeeded(currentItemData, this, playerController);

        // 2. UI 연결 해제
        droppedUIItem.RemoveSlotItem();

        // 3. 슬롯 데이터 초기화
        ClearSlot();

        previousSlot.ClearSlot();
        // 4. 아이템 재배치
        // 드롭된 아이템을 현재 슬롯에
        itemData = droppedItemData;
        HasItem = true;
        UI_Item = droppedUIItem;
        droppedUIItem.SetItem(gameObject);
        droppedUIItem.SetPosition();
        SetItem(UI_Item.myItemData);

        // 현재 아이템을 이전 슬롯에
        previousSlot.itemData = currentItemData;
        previousSlot.HasItem = true;
        previousSlot.UI_Item = currentUIItem;
        currentUIItem.SetItem(previousSlot.gameObject);
        currentUIItem.SetPosition();

        // 5. 장비 장착 처리 (이동 후)
        EquipIfNeeded(droppedItemData, this, playerController);
        EquipIfNeeded(currentItemData, previousSlot, playerController);

        Debug.Log("아이템 교환 완료");
    }

    private void MoveItemToEmptySlot(ItemData droppedItemData, UI_Item droppedUIItem, InventorySlot previousSlot, PlayerController playerController)
    {
        Debug.Log($"아이템 이동 실행: {droppedItemData.ItemName} -> {slotType} 슬롯");

        // 1. 장비 해제 처리 (이전 슬롯이 장비 슬롯인 경우)
        UnequipIfNeeded(droppedItemData, previousSlot, playerController);

        // 2. UI 연결 해제
        droppedUIItem.RemoveSlotItem();

        // 3. 이전 슬롯 초기화
        previousSlot.ClearSlot();
        // 4. 현재 슬롯에 아이템 설정
        itemData = droppedItemData;
        HasItem = true;
        UI_Item = droppedUIItem;
        droppedUIItem.SetItem(gameObject);
        droppedUIItem.SetPosition();
        SetItem(itemData);
        // 5. 장비 장착 처리 (현재 슬롯이 장비 슬롯인 경우)
        EquipIfNeeded(droppedItemData, this, playerController);

        Debug.Log("아이템 이동 완료");
    }

    private void EquipIfNeeded(ItemData equipItemData, InventorySlot slot, PlayerController playerController)
    {
        if (slot.slotType == SlotType.Equipment &&
            equipItemData.ItemType == ItemType.Equipment &&
            ((EquipmentData)equipItemData.Data) != null)
        {
            foreach(KeyValuePair<StatType, float> stat in ((EquipmentData)equipItemData.Data).Stats)
            {
                playerController.ApplyEquipment(stat.Key, stat.Value);
                Debug.Log($"[장착 완료] {equipItemData.ItemName} - {stat.Key} : +{stat.Value}");
            }
        }
    }

    private void UnequipIfNeeded(ItemData equipItemData, InventorySlot slot, PlayerController playerController)
    {
        if (slot.slotType == SlotType.Equipment &&
            equipItemData.ItemType == ItemType.Equipment &&
            ((EquipmentData)equipItemData.Data) != null)
        {
            foreach(KeyValuePair<StatType, float> stat in ((EquipmentData)equipItemData.Data).Stats)
            {
                playerController.RemoveEquipment(stat.Key, stat.Value);
                Debug.Log($"[장비 해제] {equipItemData.ItemName} - {stat.Key} : -{stat.Value}");
            }
        }
    }
}
