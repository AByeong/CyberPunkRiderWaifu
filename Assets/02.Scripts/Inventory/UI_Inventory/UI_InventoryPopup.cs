using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    public TextMeshProUGUI GoldText;
    
    private bool _isDrag = false;
    private bool _isSwapEquipment = false;


    [SerializeField]
    private List<Item> items;

    private void Awake()
    {
        Instance = this;
        
        Refresh();

        InventoryManager.Instance.OnDataChanged += Refresh;
    }

    public void RefreshGold()
    {
        GoldText.text = $"{CurrencyManager.Instance.Gold}골드";
    }

    public bool IsInventoryFull()
    {
        foreach (UI_InventorySlot slot in _slots)
        {
            if (slot.HasItem == false)
            {
                return false;
            }
        }

        return true;
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
            //내 아이템(fromSlot)을 빈 공간(toSlot)에 넣을 때
            //toSlot은 드래그 End에서 Ray hit한 Slot
            //fromSlot은 드래그 Begin에서 저장된 Slot
            int toIndex = -1;
            
            //  fromSlot     toSlot
            // Inventory -> Inventory
            // Inventory -> Equipment
            // Inventory -> Chip
            if (_fromSlot.SlotType == SlotType.Inventory)
            {
                if (toSlot.SlotType == SlotType.Inventory)
                {
                    // _slots의 멤버 UI_InvenrtorySlot을 돌며 toSlot과 같은 slot의 index를 반환
                    toIndex = _slots.FindIndex(slot => slot == toSlot); 
                }   
                else if (toSlot.SlotType == SlotType.Equipment)
                {
                    toIndex = CheckEquipmentSlotType(toSlot,toIndex);
                }
                else if (toSlot.SlotType == SlotType.Chip)
                {
                    toIndex = CheckChipSlotType(toSlot, toIndex);
                }    
            }
            // Equipment -> Inventory
            // Equipment -> Chip X
            // Equipment -> Equipment X (같은 자리에 놓는건 미리 X)
            else if (_fromSlot.SlotType == SlotType.Equipment)
            {
                if (toSlot.SlotType == SlotType.Inventory)
                {
                    toIndex = _slots.FindIndex(slot => slot == toSlot);
                    // InventoryManager.Instance.RemoveStat(_fromSlot.Item);
                    // _fromSlot.Item.IsEquipped = false;
                }
                else if (toSlot.SlotType == SlotType.Chip ||
                         toSlot.SlotType == SlotType.Equipment)
                {
                    return;
                }
            }
            // Chip -> Inventory
            // Chip -> Equipment X
            // Chip -> Chip
            else if (_fromSlot.SlotType == SlotType.Chip)
            {
                if (toSlot.SlotType == SlotType.Inventory)
                {
                    toIndex = _slots.FindIndex(slot => slot == toSlot);
                }
                else if (toSlot.SlotType == SlotType.Equipment)
                {
                    return;
                }
                else if (toSlot.SlotType == SlotType.Chip)
                {
                    toIndex = CheckChipSlotType(toSlot, toIndex);
                }
            }
                
            Debug.Log($"Index : {toIndex}");
            _fromSlot.Item.SetSlotIndex(toSlot.SlotType, toIndex);   
        }
        else
        {  
            //양쪽이 아이템이 있는 경우
            //from과 to가 같은 슬롯인 경우 무조건 X
            //Equipment <-> Equipment 무조건 X
            //Chip      <-> Equipment 무조건 X
            //Equipment <-> Chip 무조건 X
            // if (_fromSlot.Item == toSlot.Item ||
            //     _fromSlot.SlotType == SlotType.Equipment && toSlot.SlotType == SlotType.Equipment ||
            //     _fromSlot.SlotType == SlotType.Chip && toSlot.SlotType == SlotType.Equipment ||
            //     _fromSlot.SlotType == SlotType.Equipment && toSlot.SlotType == SlotType.Chip)
            // {
            //     
            // }
            
            //Chip      <-> Chip 무조건 스왑
            //Inventory <-> Inventory 무조건 스왑
            if (_fromSlot.SlotType == SlotType.Chip && toSlot.SlotType == SlotType.Chip ||
                _fromSlot.SlotType == SlotType.Inventory && toSlot.SlotType == SlotType.Inventory)
            {
                SwapSlot(toSlot);
            }
            
            //Inventory <-> Equipment : fromSlot.Item.Data의 EquipmentDataSO.EquipmentType과 toSlot.Item.Data의 SO.Type이 같을 때
            //Equipment <-> Inventory : toSlot.Item.Data의 EquipmentDataSO.EquipmentType이 두 아이템이 서로 같은 경우
            else if (_fromSlot.SlotType == SlotType.Inventory && toSlot.SlotType == SlotType.Equipment ||
                _fromSlot.SlotType == SlotType.Equipment && toSlot.SlotType == SlotType.Inventory)
            {
                if (_fromSlot.Item.Data is EquipmentDataSO equipmentDataSO)
                {
                    EquipmentDataSO dataSO = toSlot.Item.Data as EquipmentDataSO;
                    if (equipmentDataSO.EquipmentType == dataSO.EquipmentType)
                    {
                        SwapSlot(toSlot);
                        _isSwapEquipment = true;
                    }
                }
            }
            
            //Inventory <-> Chip : fromSlot.Item.Data.ItemType이 ItemType.Chip인 경우
            //Chip <-> Inventory : toSlot.Item.Data.ItemType이 ItemType.Chip인 경우
            else if (_fromSlot.SlotType == SlotType.Inventory && toSlot.SlotType == SlotType.Chip ||
                     _fromSlot.SlotType == SlotType.Chip && toSlot.SlotType == SlotType.Inventory)
            {
                if (_fromSlot.Item.Data.ItemType == ItemType.Chip)
                {
                    SwapSlot(toSlot);
                }
            }
        }
    
        if (_isSwapEquipment)
        {
            if (toSlot.HasItem)
            {
                // InventoryManager.Instance.RemoveStat(toSlot.Item);
            }
            InventoryManager.Instance.Equip(_fromSlot.Item);
            _isSwapEquipment = false;
        }
        StopDragSlot();
        
        Refresh();
    }

    private void SwapSlot(UI_InventorySlot toSlot)
    {
        int fromIndex =  _fromSlot.Item.SlotIndex;
        SlotType fromSlotType = _fromSlot.SlotType;
        _fromSlot.Item.SetSlotIndex(toSlot.SlotType, toSlot.Item.SlotIndex);
        toSlot.Item.SetSlotIndex(fromSlotType, fromIndex);
    }

    private int CheckEquipmentSlotType(UI_InventorySlot toSlot, int toIndex)
    {
        if (_fromSlot.Item.Data is EquipmentDataSO dataSO)
        {
            if (toSlot is UI_EquipmentSlot toEquipmentSlot)
            {
                if (dataSO.EquipmentType != toEquipmentSlot.EquipmentType) return -1;
                
                toIndex = _equipmentSlots.FindIndex(slot => slot == toSlot);
                _isSwapEquipment = true;
                return toIndex;
            }
        }
        return -1;
    }

    private int CheckChipSlotType(UI_InventorySlot toSlot, int toIndex)
    {
        if (_fromSlot.Item.Data is ChipDataSO chipDataSO)
        {
            if (toSlot is UI_ChipSlot toChipSlot)
            {
                toIndex = _chipSlots.FindIndex(slot => slot == toSlot);
                return toIndex;
            }    
        }
        

        return -1;
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

        foreach (var slot in _equipmentSlots)
        { 
            slot.SetItem(null);
        }
        foreach (var slot in _chipSlots)
        {
            slot.SetItem(null);
        }

        items = InventoryManager.Instance.Items;
        
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

        GoldText.text = $"{CurrencyManager.Instance.Gold.ToString()}골드";
    }
}
