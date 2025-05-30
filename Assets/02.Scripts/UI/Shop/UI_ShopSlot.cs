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
        Price = int.Parse(PriceText.text);
    }

    public void OnSellItems()
    {
        if (CurrencyManager.Instance.Gold < Price)
        {
            Debug.Log("돈이 없어서 상점에서 아이템을 못사 ㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋ");
            return;
        }

        if (UI_InventoryPopup.Instance.IsInventoryFull() == true)
        {
            Debug.Log("가방이 꽉차서 상점에서 아이템을 못사 ㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇ");
            return;
        }

        Item item = null;
        switch (SlotType)
        {
            case EShopSlotType.Head:
                item = ItemCreateManager.Instance.CreateHead();
                break;
            case EShopSlotType.Armor:
                item = ItemCreateManager.Instance.CreateArmor();
                break;
            case EShopSlotType.Boots:
                item = ItemCreateManager.Instance.CreateBoots();
                break;
            case EShopSlotType.Weapon:
                item = ItemCreateManager.Instance.CreateWeapon();
                break;
            case EShopSlotType.Chip:
                item = ItemCreateManager.Instance.CreateChip();
                break;
            case EShopSlotType.Item1:
                // 소모아이템 1 추기
                break;
            case EShopSlotType.Item2:
                // 소모아이템 2 추가
                break;
        }

        if (InventoryManager.Instance.Add(item) == true)
        {
            Price *= 2;
            PriceText.text = Price.ToString();
        }
    }
}


