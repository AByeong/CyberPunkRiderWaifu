using System;
using System.Collections.Generic;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UI;


public enum EShopSlotType
{
    Head,
    Armor,
    Boots,
    Weapon,
    Chip,
    Item1,
    Item2,
    Item3,
    Sell,
    Count
}
public class UI_ShopPopup : Popup
{
    public List<UI_ShopSlot> ShopSlots;
    public Image BuyButtonImage;
    public Image SellButtonImage;
    public Sprite OnSprite;
    public Sprite OffSprite;

    public GameObject BuyPanel;
    public GameObject SellPanel;
    private void Start()
    {
        // PriceReset();
    }

    public void OnBuyButtonClick()
    {
        BuyButtonImage.sprite = OnSprite;
        SellButtonImage.sprite = OffSprite;
        BuyPanel.SetActive(true);
        SellPanel.SetActive(false);
    }

    public void OnSellButtonClick()
    {
        SellButtonImage.sprite = OnSprite;
        BuyButtonImage.sprite = OffSprite;
        BuyPanel.SetActive(false);
        SellPanel.SetActive(true);
    }

    public void PriceReset()
    {
        foreach (var slot in ShopSlots)
        {
            slot.PriceText.text = $"{slot.Price.ToString()}";
        }
    }

   
}
