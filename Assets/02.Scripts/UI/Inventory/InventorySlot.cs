using JY;
using UnityEngine;
using UnityEngine.EventSystems;
public enum SlotType // 인스펙터에서 설정
{
    Inventory, // 일반 인벤토리
    Equipment // 장비 슬롯
}

public class InventorySlot : MonoBehaviour, IDropHandler, IDragHandler, IPointerEnterHandler, IEndDragHandler, IBeginDragHandler
{
    public SlotType SlotType;
    public UI_Itembase[] Items_UI;

    public Item Item;
    public bool HasItem;

    public EquipmentType equipmentType; // SlotType이 Equipment일 경우 사용

    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;

    private void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        AllItemHide();
        if (HasItem)
        {
            Items_UI[(int)Item.Type].Show(Item);
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.interactable = false;
        transform.SetParent(_canvas.transform, false);
    }
    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }
    public virtual void OnDrop(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerDrag.name + " " + gameObject.name);
        Debug.Log(eventData.pointerDrag.GetComponent<InventorySlot>().Item.name);

        Item droppedItem = eventData.pointerDrag.GetComponent<InventorySlot>().Item;
        // Item droppedItem = new Item(); // 박제

        // 아이템 이동/교환 처리
        Debug.Log("드롭 처리: 아이템 이동 또는 교환을 진행합니다.");
        SetData(droppedItem);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // throw new NotImplementedException();
    }

    private void AllItemHide()
    {
        foreach(UI_Itembase UI_Item in Items_UI)
        {
            UI_Item.gameObject.SetActive(false);
        }
    }

    public virtual void SetData(Item newItem)
    {
        Item = newItem;
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
        SetData(droppedItem);
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
