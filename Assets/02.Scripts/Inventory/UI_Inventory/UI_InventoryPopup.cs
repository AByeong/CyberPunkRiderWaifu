using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using JY;

public class UI_InventoryPopup : Popup
{

    public static UI_InventoryPopup Instance;
    
    [SerializeField]
    private List<UI_InventorySlot> _slots;
    private List<UI_EquipmentSlot> _equipmentSlots;
    private List<UI_ChipSlot> _chipSlots;
    
    public UI_InventorySlot DragSlot;
    private UI_InventorySlot _fromSlot;
    
    
    private bool _isDrag = false;


    private void Awake()
    {
        Instance = this;
        
        Refresh();

        InventoryManager.Instance.OnDataChanged += Refresh;
    }
    public void StartDragSlot(UI_InventorySlot fromSlot)
    {
        if (!fromSlot.HasItem) return;
        
        Debug.Log($"from: {fromSlot.gameObject.name}");
        
        DragSlot.gameObject.SetActive(true);
        
        DragSlot.SetItem(fromSlot.Item);

        _fromSlot = fromSlot;

        _isDrag = true;
    }

    public void StopDragSlot()
    {
        _isDrag = false;
        
        DragSlot.gameObject.SetActive(false);
    }

    public void SwapSlotItem(UI_InventorySlot toSlot)
    {
        if (_fromSlot == null || !_fromSlot.HasItem)
        {
            return;
        }
        
        if(!toSlot.HasItem)
        {
            int toIndex = _slots.FindIndex(slot => slot == toSlot);
            _fromSlot.Item.SetSlotIndex(toIndex);   
        }
        else
        {
            int fromIndex =  _fromSlot.Item.SlotIndex;
            _fromSlot.Item.SetSlotIndex(toSlot.Item.SlotIndex);
            toSlot.Item.SetSlotIndex(fromIndex);
        }
        
        StopDragSlot();
        
        Refresh();
    }

    public void EquipItem(UI_EquipmentSlot toSlot)
    {

        if (!slot)
        {
            //slot.EquipmentType == _fromSlot.Item.Data.
        }
    }

    public void SetChip(UI_ChipSlot toSlot)
    {
        
    }

    private void Update()
    {
        if (_isDrag)
        {
            DragSlot.transform.position = Input.mousePosition;
        }
    }
    
    
    public override void OpenPopup()
    {
        UIManager.Instance.PlayerInput.ReleaseControl();
        UIManager.Instance.ESCisClose = true;
        base.OpenPopup();
    } 
    
    public override void ClosePopup() 
    {
        UIManager.Instance.PlayerInput.GainControl();
        
        UIManager.Instance.ESCisClose = false;
        base.ClosePopup();
    }
    


    
    private void Refresh()
    {
        foreach (var slot in _slots)
        {
            slot.SetItem(null);
        }
        
        List<Item> items = InventoryManager.Instance.Items;
        
        // 인덱스 있는 아이템 먼저 채우고,
        foreach (Item item in items)
        {
            if (item.SlotIndex < 0) continue;
            
            _slots[item.SlotIndex].SetItem(item);
        }

        // 인덱스 없는 아이템은 앞에서부터 채운다.
        foreach (Item item in items)
        {
            if (item.SlotIndex >= 0) continue;

            int emptyIndex = _slots.FindIndex((slot) => !slot.HasItem);
            
            _slots[emptyIndex].SetItem(item);
            item.SetSlotIndex(emptyIndex);
        }
    }
}
