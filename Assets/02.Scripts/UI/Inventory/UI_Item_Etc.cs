using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Item_Etc : UI_Itembase
{
    public ItemEtc Data;
    public override void OnEndDrag(PointerEventData eventData)
    {
        
    }

    

    public override void Init(Item item, GameObject inventorySlot)
    {
        base.SetItem(item); // Data도 여기서 설정됨
        InventorySlot = inventorySlot;
        OriginalSlot = inventorySlot;
    }



    public override void SetItem(Item item)
    {
        base.SetItem(item);

        // Chip 데이터 추출
        Data = item is ItemEtc etc ? etc : null;
    }

    public void RemoveSlotItem()
    {
        base.RemoveSlotItem(); // 기본 처리 수행
        Data = null;           // ItemChip 전용 데이터 정리
    }


}