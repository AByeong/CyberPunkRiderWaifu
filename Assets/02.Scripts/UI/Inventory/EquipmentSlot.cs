using Gamekit3D;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour, IDropHandler
{
    public Image Icon;
    public Item EquippedItem;
    public EquipmentType _equipmentType; // Inspector에서 지정해주세요
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        if (!eventData.pointerDrag.TryGetComponent<UI_Item>(out var uiItem)) return;

        Item draggedItem = uiItem.MyItem;

        if (draggedItem == null || draggedItem.ItemType != ItemType.Equipment) return;

        var equipType = draggedItem.EquipmentData.EquipmentType;
        if (_equipmentType != equipType) return;
        
        EquipItem(draggedItem);
        
        uiItem.SetItem(gameObject);
        uiItem.SetPosition();
        
        //InventorySlot originSlot = eventData.pointerDrag?.GetComponent<InventorySlot>();

    }

    public void EquipItem(Item item)
    {
        if (item == null || item.ItemType != ItemType.Equipment)
        {
            Debug.LogWarning("잘못된 아이템이 장착 시도됨.");
            return;
        }
        PlayerController playerController = GameManager.Instance.player;
        
        if (EquippedItem != null)
        {
            foreach(var stat in EquippedItem.EquipmentData.Stats)
            {
                playerController.RemoveEquipment(stat.Key, stat.Value);    
            }
        }

        EquippedItem = item;
        if (Icon != null)
            Icon.sprite = item.Icon;
        
        // Player에게 Prefab 붙여주기 및 스탯 적용
        foreach(var stat in EquippedItem.EquipmentData.Stats)
        {
            playerController.ApplyEquipment(stat.Key, stat.Value);    
            Debug.Log($"[장착 완료] {stat.Key.ToString()} : {stat.Value}");
        }

    }
}