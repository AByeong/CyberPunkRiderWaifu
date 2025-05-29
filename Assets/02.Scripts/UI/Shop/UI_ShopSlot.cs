using System;
using TMPro;
using Unity.AppUI.UI;
using UnityEngine;

public class UI_ShopSlot : MonoBehaviour
{
    public EShopSlotType SlotType;
    public Button SlotButton;
    public Item SellingItem;
    public TextMeshProUGUI PriceText;
    public int Price { get; private set; }

    public void Start()
    {
        Price = int.Parse(PriceText.ToString());
    }

    public void OnSellItems()
    {
        
    }
}


