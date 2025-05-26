using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Itembase : MonoBehaviour, IDragHandler, IPointerEnterHandler, IEndDragHandler, IBeginDragHandler
{
    public GameObject InventorySlot;
    public GameObject OriginalSlot;
    protected RectTransform _rectTransform;
    private Transform _originalParent;

    // 드래그 시작 시 원래 위치 저장
    private Vector2 _originalPosition; 
    private bool _wasDroppedOnValidSlot;
    
    private Canvas _canvas;

    public Item Item;
    
    public virtual void Init(Item item, GameObject inventorySlot)
    {
        
    }
    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작 시 원래 위치와 부모 저장
        _originalPosition = _rectTransform.anchoredPosition;
        _originalParent = transform.parent;
        _wasDroppedOnValidSlot = false;

        transform.SetParent(_canvas.transform);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }
    public virtual void SetItem(Item item)
    {
        Item = item;

        // 기본 아이콘 변경 등 UI 갱신
        if (item != null && item.Icon != null)
        {
            GetComponent<Image>().sprite = item.Icon;
        }
        else
        {
            GetComponent<Image>().sprite = null; // 아이템 없을 때
        }

        // InventorySlot에 있는 Item 데이터도 업데이트
        if (InventorySlot != null)
        {
            InventorySlot.GetComponent<InventorySlot>().Item = item;
        }
    }
    public virtual void OnEndDrag(PointerEventData eventData)
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
                var targetUIItem = targetSlot.GetComponentInChildren<UI_Itembase>();

                if (targetUIItem != null)
                {
                    // 데이터만 교환
                    Item temp = this.Item;
                    this.SetItem(targetUIItem.Item);
                    targetUIItem.SetItem(temp);

                    Debug.Log("아이템 데이터 스왑 완료");
                }

                return;
            }
        }

        // 유효하지 않은 곳에 드롭된 경우 원래 위치로 복귀
        ReturnToOriginalPosition();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 툴팁이나 기타 UI 표시 가능
    }

    public virtual void RemoveSlotItem()
    {
        if (InventorySlot == null) return;

        // 데이터 제거
        InventorySlot.GetComponent<InventorySlot>().Item = null;
        Item = null;

        // UI 초기화
        Image image = GetComponent<Image>();
        if (image != null)
        {
            image.sprite = null;
        }

        // 위치 복귀 또는 비활성화 처리도 가능
        _rectTransform.anchoredPosition = Vector2.zero;
    }
    public void SetPosition()
    {
        transform.SetParent(InventorySlot.transform);
        _rectTransform.anchoredPosition = Vector2.zero;
    }
    
    private void ReturnToOriginalPosition()
    {
        // 원래 부모로 돌아가기
        transform.SetParent(_originalParent);

        // 원래 위치로 돌아가기
        _rectTransform.anchoredPosition = _originalPosition;
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
}

