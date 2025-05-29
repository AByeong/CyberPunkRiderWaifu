using System;
using System.Collections.Generic;
using Unity.AppUI.UI;
using UnityEngine;

public enum EShopSlotType
{
    Head,
    Armor,
    Boots,
    Weapon,
    Chip,
    Item1,
    Item2,
    Sell,
    Count
}
public class UI_ShopPopup : Popup
{
    public List<UI_ShopSlot> ShopSlots;

    private void Start()
    {
        PriceReset();
    }

    public void PriceReset()
    {
        foreach (var slot in ShopSlots)
        {
            slot.Price = 1000;
            slot.PriceText.text = $"{slot.Price.ToString()}";
        }
    }
}
