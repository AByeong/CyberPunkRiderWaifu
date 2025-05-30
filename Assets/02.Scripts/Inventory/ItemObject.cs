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

    private bool _isTaked = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == true)
        {
            // Equipment, Chip, Etc 일 경우 (Etc는 추가될 소비아이템) -> 소비아이템 인벤에 들어오는 형식 아니면 추후 수정
            if (_isTaked == true) return;
            if(item == null)
                Debug.LogError("Item이 Null임");

            InventoryManager.Instance.Add(item);
            
            _isTaked = true;

            // 파티클과 사운드 중지
            if (_particleSystem != null)
                _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (_audioSource != null)
                _audioSource.Stop();
            Destroy(transform.root.gameObject, 0.1f);
            
            //InventoryManager.Instance.
            // Gold는 추후 UI 생기면 거기에 +
        }
    }
    private void Start()
    {
        _collider = GetComponent<SphereCollider>();
        _particleSystem = GetComponent<ParticleSystem>();
        _audioSource = GetComponent<AudioSource>();

        CreateRandomItem();
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
