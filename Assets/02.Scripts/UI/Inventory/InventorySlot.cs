using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Item currentItem;
    public Image Icon;
    public Image DragImage;
    private Canvas _parentCanvas;

    private void Start()
    {
        _parentCanvas = GetComponentInParent<Canvas>();
        if (currentItem != null)
        {
            Icon.sprite = currentItem.Icon;
            DragImage.sprite = currentItem.Icon;
        }
    }

    public void AddItem(Item newItem)
    {
        currentItem = newItem;
        Icon.sprite = newItem.Icon;
        Icon.enabled = true;
    }

    public void ClearSlot()
    {
        currentItem = null;
        Icon.sprite = null;
        Icon.enabled = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin");
        if (currentItem == null) return;
        
        DragImage.sprite = Icon.sprite;
        DragImage.enabled = true;
        DragImage.transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Drag");
        if (DragImage != null)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentCanvas.transform as RectTransform,
                eventData.position,
                _parentCanvas.worldCamera,
                out pos);
            DragImage.rectTransform.anchoredPosition = pos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End");
        DragImage.enabled = false;
    }
}
