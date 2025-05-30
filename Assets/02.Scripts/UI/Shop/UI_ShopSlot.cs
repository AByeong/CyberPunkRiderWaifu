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
        switch (SlotType)
        {
            case EShopSlotType.Head:
                Item item = ItemCreateManager.Instance.CreateHead();
                if (CurrencyManager.Instance.Gold < Price)
                {
                    Debug.Log("돈이 없어서 상점에서 아이템을 못사 ㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋ");
                    return;
                }
                if (InventoryManager.Instance.Add(item) == false)
                {
                    Debug.Log("가방이 꽉차서 상점에서 아이템을 못사 ㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇ");
                    return;
                }
                break;
            case EShopSlotType.Armor:
                break;
            case EShopSlotType.Boots:
                break;
            case EShopSlotType.Weapon:
                break;
            case EShopSlotType.Chip:
                break;
            case EShopSlotType.Item1:
                break;
            case EShopSlotType.Item2:
                break;
            case EShopSlotType.Sell:
                break;
            case EShopSlotType.Count:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        Price *= 2;
        
    }
}


