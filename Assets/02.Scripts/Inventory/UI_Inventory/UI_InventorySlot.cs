using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public enum SlotType // 인스펙터에서 설정
{
    Inventory, // 일반 인벤토리
    Equipment, // 장비 슬롯
    Chip
}

public class UI_InventorySlot : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public SlotType SlotType;

    [Header("UI")]
    public Image IconImageUI;
    public Item Item;

    public bool HasItem => Item != null;

    private void Start()
    {
    }

    private void Update()
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        UI_InventoryPopup.Instance.StartDragSlot(this);
        Color color = IconImageUI.color;
        color.a = 0.5f;
        IconImageUI.color = color;
    }

    public void OnDrag(PointerEventData eventData)
    {

    }


    public virtual void OnDrop(PointerEventData eventData)
    {
        // 마우스 위치에 있는 UI 오브젝트 찾기
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);


        foreach(RaycastResult result in raycastResults)
        {
            UI_InventorySlot targetSlot = result.gameObject.GetComponentInParent<UI_InventorySlot>();
            if (targetSlot != null)
            {
                Debug.Log($"to: {targetSlot.gameObject.name}");

                UI_InventoryPopup.Instance.SwapSlotItem(targetSlot);

                // if(targetSlot.SlotType == SlotType.Inventory)
                // {
                //     Debug.Log("targetSlot == Inventory");
                //     UI_InventoryPopup.Instance.SwapSlotItem(targetSlot);
                // }
                // else if (targetSlot.SlotType == SlotType.Equipment)
                // {
                //     Debug.Log("targetSlot == Equipment");
                //     UI_InventoryPopup.Instance.EquipItem((UI_EquipmentSlot)targetSlot);
                // }
                // else if (targetSlot.SlotType == SlotType.Chip)
                // {
                //     Debug.Log("targetSlot == Chip");
                //     UI_InventoryPopup.Instance.SetChip((UI_ChipSlot)targetSlot);
                // }
                break;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        UI_InventoryPopup.Instance.StopDragSlot();
        Color color = IconImageUI.color;
        color.a = 1.0f;
        IconImageUI.color = color;
    }

    public virtual void SetItem(Item item)
    {
        if (item == null)
        {
            ClearItem();
            return;
        }

        Item = item;
        Set();
    }
    protected virtual void ClearItem()
    {
        IconImageUI.sprite = null;
        Item = null;
    }

    private void Set()
    {
        // TODO: UI 세팅

        if (!HasItem) return;

        IconImageUI.sprite = Item.Data.Icon;
    }
}
