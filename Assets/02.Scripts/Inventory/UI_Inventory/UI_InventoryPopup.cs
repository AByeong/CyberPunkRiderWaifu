using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using JY;

public class UI_InventoryPopup : Popup
{

    public static UI_InventoryPopup Instance;
    
    [SerializeField]
    private List<UI_InventorySlot> _slots;
    [SerializeField]
    private List<UI_EquipmentSlot> _equipmentSlots;
    [SerializeField]
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
            Debug.Log("Swap Return");
            return;
        }
        
        if(toSlot.HasItem == false)
        {
            //toSlot은 드래그 End에서 Ray hit한 Slot
            //fromSlot은 드래그 Begin에서 저장된 Slot
            int toIndex = -1;
            if (toSlot.SlotType == SlotType.Inventory)
            {
                // _slots의 멤버 UI_InvenrtorySlot을 돌며 toSlot과 같은 slot의 index를 반환
                toIndex = _slots.FindIndex(slot => slot == toSlot); 
            }   
            else if (toSlot.SlotType == SlotType.Equipment)
            {
                toIndex = CheckEquipmentSlotType(toSlot,toIndex);
            }
                
            Debug.Log($"Index : {toIndex}");
            _fromSlot.Item.SetSlotIndex(toSlot.SlotType, toIndex);   
        }
        else
        {  
            int fromIndex =  _fromSlot.Item.SlotIndex;
            SlotType fromSlotType = _fromSlot.SlotType;

            CheckEquipmentSlotType(toSlot,_equipmentSlots.FindIndex(slot => slot == toSlot));
            
            
            
            if (toSlot.SlotType == SlotType.Inventory)
            {
                // _slots의 멤버 UI_InvenrtorySlot을 돌며 toSlot과 같은 slot의 index를 반환
                toIndex = _slots.FindIndex(slot => slot == toSlot); 
            }   
            else if (toSlot.SlotType == SlotType.Equipment)
            {
                toIndex = CheckEquipmentSlotType(toSlot,toIndex);
            }
            
            
            _fromSlot.Item.SetSlotIndex(toSlot.SlotType, toSlot.Item.SlotIndex);
            toSlot.Item.SetSlotIndex(slotType, fromIndex);
        }
    
        if (toSlot.SlotType == SlotType.Equipment)
        {
            if (toSlot.HasItem == true)
            {
                InventoryManager.Instance.RemoveStat(toSlot.Item);
            }
            InventoryManager.Instance.AddStat(_fromSlot.Item);
        }
        StopDragSlot();
        
        Refresh();
    }

    private void SwapSlot(UI_InventorySlot toSlot, int toIndex)
    {
        
    }

    private int CheckEquipmentSlotType(UI_InventorySlot toSlot, int toIndex)
    {
        if (_fromSlot.Item.Data is EquipmentDataSO dataSO)
        {
            if (toSlot is UI_EquipmentSlot toEquipmentSlot)
            {
                if (dataSO.EquipmentType != toEquipmentSlot.EquipmentType) return -1;
                
                toIndex = _equipmentSlots.FindIndex(slot => slot == toSlot);
                return toIndex;
            }
        }
        return -1;
    }

    private int CheckChipSlotType(UI_InventorySlot toSlot, int toIndex)
    {
        
    }
    
    public void EquipItem(UI_EquipmentSlot toSlot)
    {
        StopDragSlot();
        
        Refresh();
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E");
            foreach (UI_InventorySlot slot in _slots)
            {
                if (slot.HasItem == false)
                {
                    slot.SetItem(InventoryManager.Instance.Items[1]);
                }
            }
            Refresh();
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



    private void SetItemToSlot(Item item)
    {
        switch (item.SlotType)
        {
            case SlotType.Inventory:
                _slots[item.SlotIndex].SetItem(item);
                break;
            case SlotType.Equipment:
                _equipmentSlots[item.SlotIndex].SetItem(item);
                break;
            case SlotType.Chip:
                _chipSlots[item.SlotIndex].SetItem(item);
                break;
        }
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
            SetItemToSlot(item);
        }

        // 인덱스 없는 아이템은 앞에서부터 채운다.
        foreach (Item item in items)
        {
            if (item.SlotIndex >= 0) continue;

            int emptyIndex = _slots.FindIndex((slot) => !slot.HasItem);
            
            _slots[emptyIndex].SetItem(item);
            item.SetSlotIndex(SlotType.Inventory, emptyIndex);
        }
    }
}
