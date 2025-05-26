using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Item_Chip : UI_Itembase
{
    public ItemChip Data;
    public override void OnEndDrag(PointerEventData eventData)
    {
        
    }

    

    public override void Init(Item item, GameObject inventorySlot)
    {
        base.Init(item, inventorySlot);
    }
    public override void SetItem(Item item)
    {
        base.SetItem(item);

        // Chip 데이터 추출
        Data = item is ItemChip chip ? chip : null;
    }

    public override void RemoveSlotItem()
    {
        base.RemoveSlotItem(); // 기본 처리 수행
        Data = null;           // ItemChip 전용 데이터 정리
    }


}