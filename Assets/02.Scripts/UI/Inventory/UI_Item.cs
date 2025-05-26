using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Item : MonoBehaviour, IDragHandler, IPointerEnterHandler, IEndDragHandler, IBeginDragHandler
{
    private RectTransform _rectTransform;
    private Canvas _canvas;
    public Item MyItem;
    public GameObject InventorySlot;
    public GameObject OriginalSlot;
    
    // 드래그 시작 시 원래 위치 저장
    private Vector2 _originalPosition;
    private Transform _originalParent;
    private bool _wasDroppedOnValidSlot = false;
    
    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void Init(Item item, GameObject inventorySlot)
    {
        MyItem = item;
        OriginalSlot = inventorySlot;
        InventorySlot = inventorySlot;
        inventorySlot.GetComponent<InventorySlot>().item = item;
        if (item != null &&item.Icon != null)
        {
            GetComponent<Image>().sprite = item.Icon;
        }
        SetItem(inventorySlot);
    }
    
    public void SetPosition()
    {
        transform.SetParent(InventorySlot.transform);
        _rectTransform.anchoredPosition = Vector2.zero;
    }
    
    public void SetItem(GameObject slot)
    {
        InventorySlot = slot;
        InventorySlot.GetComponent<InventorySlot>().item = MyItem;
    }

    public void RemoveSlotItem()
    {
        if(InventorySlot == null) return;
        InventorySlot.GetComponent<InventorySlot>().item = null;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작 시 원래 위치와 부모 저장
        _originalPosition = _rectTransform.anchoredPosition;
        _originalParent = transform.parent;
        _wasDroppedOnValidSlot = false;
        
        transform.SetParent(_canvas.transform);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        
        Debug.Log($"드래그 시작: {MyItem.ItemName}");
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        
        // 유효한 슬롯에 드롭되었는지 확인
        GameObject droppedObject = eventData.pointerCurrentRaycast.gameObject;
        
        if (droppedObject != null)
        {
            // 드롭된 오브젝트나 그 부모 중에 InventorySlot이 있는지 확인
            InventorySlot targetSlot = GetInventorySlotFromGameObject(droppedObject);
            
            if (targetSlot != null)
            {
                Debug.Log($"유효한 슬롯에 드롭: {targetSlot.slotType}");
                _wasDroppedOnValidSlot = true;
                // InventorySlot의 OnDrop이 처리할 것임
                return;
            }
        }
        
        // 유효하지 않은 곳에 드롭된 경우 원래 위치로 복귀
        Debug.Log($"유효하지 않은 곳에 드롭됨. 원래 위치로 복귀: {MyItem.ItemName}");
        ReturnToOriginalPosition();
    }
    
    private InventorySlot GetInventorySlotFromGameObject(GameObject obj)
    {
        // 현재 오브젝트에서 InventorySlot 확인
        InventorySlot slot = obj.GetComponent<InventorySlot>();
        if (slot != null) return slot;
        
        // 부모들을 순회하면서 InventorySlot 찾기
        Transform current = obj.transform;
        while (current != null)
        {
            slot = current.GetComponent<InventorySlot>();
            if (slot != null) return slot;
            current = current.parent;
        }
        
        return null;
    }
    
    private void ReturnToOriginalPosition()
    {
        // 원래 부모로 돌아가기
        transform.SetParent(_originalParent);
        
        // 원래 위치로 돌아가기
        _rectTransform.anchoredPosition = _originalPosition;
        
        Debug.Log($"원래 위치로 복귀 완료: {MyItem.ItemName}");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 툴팁이나 기타 UI 표시 가능
    }
}