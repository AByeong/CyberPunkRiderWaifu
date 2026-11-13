using System;
using UnityEngine;
using Random = System.Random;

public class ItemObject : MonoBehaviour
{
    private SphereCollider _collider;
    private Item item;
    private ParticleSystem _particleSystem;
    private AudioSource _audioSource;
    public ItemRarity Rarity;
    public DropItemType DropType;

    private int _gold = 0;
    private bool _consumableItem1 = false;
    private bool _consumableItem2 = false;
    private bool _consumableItem3 = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == true)
        {
            // Equipment, Chip, Etc 일 경우 (Etc는 추가될 소비아이템) -> 소비아이템 인벤에 들어오는 형식 아니면 추후 수정
            // Etc/Gold 드롭 모두 굴린 결과(골드 또는 소모품)를 그대로 지급한다
            // (기존에는 Etc에서 나온 골드, Gold에서 나온 소모품이 버려졌음)
            if (DropType == DropItemType.Etc || DropType == DropItemType.Gold)
            {
                if (_gold != 0)
                {
                    CurrencyManager.Instance.Add(CurrencyType.Gold, _gold);
                    DeliveryManager.Instance.GoldCount += _gold;
                    if (UI_InventoryPopup.Instance != null) UI_InventoryPopup.Instance.RefreshGold();
                }

                if (_consumableItem1 == true)
                {
                    ConsumableItemManager.Instance.AddItem((int)EConsumableItemType.HPRecovery, 1);
                }
                else if (_consumableItem2 == true)
                {
                    ConsumableItemManager.Instance.AddItem((int)EConsumableItemType.UltCooldown, 1);
                }
                else if (_consumableItem3 == true)
                {
                    ConsumableItemManager.Instance.AddItem((int)EConsumableItemType.AllSkillCooldown, 1);
                }
            }
            else
            {
                if (item == null) return;
                Debug.Log($"아이템 습득: {item.Data.ItemName}");
                // 가방이 가득 차 있으면 줍지 않고 바닥에 남겨둔다 (기존에는 아이템이 사라졌음)
                if (!InventoryManager.Instance.Add(item)) return;
            }

                // 파티클과 사운드 중지
                if (_particleSystem != null)
                    _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                if (_audioSource != null)
                    _audioSource.Stop();
                //Destroy(transform.root.gameObject, 0.1f);
                //SoundManager.Instance.Play(SoundType.ItemRootingSound);
                gameObject.SetActive(false);
            
                //InventoryManager.Instance.
                // Gold는 추후 UI 생기면 거기에 +
        }
    }
    private void Start()
    {
        _collider = GetComponent<SphereCollider>();
        _particleSystem = GetComponent<ParticleSystem>();
        _audioSource = GetComponent<AudioSource>();
    }

    public void Init(DropItemType type)
    {
        DropType = type;
        gameObject.SetActive(true);
        if (DropType == DropItemType.Item)
        {
            CreateRandomItem();    
        }

        else
        {
            _gold = 0;
            _consumableItem1 = false;
            _consumableItem2 = false;
            _consumableItem3 = false;

            if (DropType == DropItemType.Gold)
            {
                // 골드 드롭은 항상 골드를 준다
                _gold = UnityEngine.Random.Range(200, 400);
            }
            else
            {
                CreateEtcItem();
            }
        }
    }

    private void CreateEtcItem()
    {
        int randomNumber = UnityEngine.Random.Range(0, 10);

        if (randomNumber < 2)
        {
            _gold = UnityEngine.Random.Range(200, 400);
        }
        else if (randomNumber < 4)
        {
            _consumableItem1 = true;
        }
        else if (randomNumber < 6)
        {
            _consumableItem2 = true;
        }
        else if (randomNumber < 8)
        {
            _consumableItem3 = true;
        }
    }

    private void CreateRandomItem()
    {
        int randomNumber = UnityEngine.Random.Range(0, 10);

        if (randomNumber < 2)
        {
            item = ItemCreateManager.Instance.CreateWeapon(Rarity);
        }
        else if (randomNumber < 4)
        {
            item = ItemCreateManager.Instance.CreateHead(Rarity);
        }
        else if (randomNumber < 6)
        {
            item = ItemCreateManager.Instance.CreateArmor(Rarity);
        }
        else if (randomNumber < 8)
        {
            item = ItemCreateManager.Instance.CreateBoots(Rarity);
        }
        else if (randomNumber < 10)
        {
            item = ItemCreateManager.Instance.CreateChip(Rarity);
        }
    }
}
