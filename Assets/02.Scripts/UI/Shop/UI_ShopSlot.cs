using System;
using TMPro;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ShopSlot : MonoBehaviour, IDropHandler
{
    public EShopSlotType SlotType;
    public Button SlotButton;
    public Item SellingItem;
    public TextMeshProUGUI PriceText;
    public int Price { get; private set; }

    private ItemRarity MakeRandomRarity()
    {
        // 10% 확률로 유니크, 20% 확률로 레어, 나머지는 노말
        int randomValue = UnityEngine.Random.Range(0, 100);
        if (randomValue < 10)
            return ItemRarity.Unique;
        else if (randomValue < 30)
            return ItemRarity.Rare;
        else
            return ItemRarity.Normal;
    }
    public void Start()
    {
        Price = 1000;
    }

    public void SellItem(Item item)
    {
        CurrencyManager.Instance.Add(CurrencyType.Gold, 1000);
        InventoryManager.Instance.Remove(item);
        Debug.Log($"골드 : {CurrencyManager.Instance.Gold}");
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
                item = ItemCreateManager.Instance.CreateHead(MakeRandomRarity());
                break;
            case EShopSlotType.Armor:
                item = ItemCreateManager.Instance.CreateArmor(MakeRandomRarity());
                break;
            case EShopSlotType.Boots:
                item = ItemCreateManager.Instance.CreateBoots(MakeRandomRarity());
                break;
            case EShopSlotType.Weapon:
                item = ItemCreateManager.Instance.CreateWeapon(MakeRandomRarity());
                break;
            case EShopSlotType.Chip:
                item = ItemCreateManager.Instance.CreateChip(MakeRandomRarity());
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
            CurrencyManager.Instance.TryConsume(CurrencyType.Gold, Price);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (SlotType != EShopSlotType.Sell) return;
        
        UI_InventorySlot draggedSlot = eventData.pointerDrag?.GetComponent<UI_InventorySlot>();

        if (draggedSlot != null)
        {
            if(draggedSlot.HasItem == true)
            {
                SellItem(draggedSlot.Item);
            }
        }
    }
}


