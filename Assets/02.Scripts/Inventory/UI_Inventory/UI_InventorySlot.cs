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

public class UI_InventorySlot : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public SlotType SlotType;

    [Header("UI")]
    public Image IconImageUI;
    public Item Item;

    public bool HasItem => Item != null;
    public bool IsSold = false;
    public void OnBeginDrag(PointerEventData eventData)
    {
        UI_InventoryPopup.Instance.StartDragSlot(this);
        Color color = IconImageUI.color;
        color.a = 0.5f;
        IconImageUI.color = color;
        IsSold = false;
    }

    public void OnDrag(PointerEventData eventData)
    {

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (HasItem == false) return;
        UI_ItemInspector.Instance.Hovered(Item);

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        UI_ItemInspector.Instance.Exit();
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
                break;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        UI_InventoryPopup.Instance.StopDragSlot();
        if (IsSold == true) return;

        if (eventData.pointerEnter == null)
        {
            UI_CheckDropItem.Instance.TryDropItem(this);    
        }
        // if (eventData.pointerEnter == null)
        // {
        //     if (HasItem)
        //     {
        //         InventoryManager.Instance.Remove(Item);
        //     }
        // }
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
        if (IconImageUI != null)
        {
            IconImageUI.sprite = null;
            Color _color = IconImageUI.color;
            _color.a = 0;
            IconImageUI.color = _color;
        }
        Item = null;
    }

    private void Set()
    {
        // TODO: UI 세팅

        if (!HasItem) return;

        if (IconImageUI != null)
        {
            IconImageUI.sprite = Item.Data.Icon;
            Color _color = IconImageUI.color;
            _color.a = 1;
            IconImageUI.color = _color;
        }
    }

}
