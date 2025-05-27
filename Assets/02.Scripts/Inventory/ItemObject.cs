using System;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    private SphereCollider _collider;
    private ItemBaseDataSO _itemBaseDataSo;
    private ParticleSystem _particleSystem;
    private AudioSource _audioSource;

    private bool _isTaked = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == true)
        {
            // Equipment, Chip, Etc 일 경우 (Etc는 추가될 소비아이템) -> 소비아이템 인벤에 들어오는 형식 아니면 추후 수정
            if (_isTaked == true) return;
            
            Item item = new Item(_itemBaseDataSo);
            InventoryManager.Instance.Add(item);
            
            _isTaked = true;

            // 파티클과 사운드 중지
            if (_particleSystem != null)
            {
                _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            if (_audioSource != null)
            {
                _audioSource.Stop();
            }
            Destroy(transform.root.gameObject, 0.1f);
            //InventoryManager.Instance.
            // Gold는 추후 UI 생기면 거기에 +
        }
    }
    void Start()
    {
        _collider = GetComponent<SphereCollider>();
        _particleSystem = GetComponent<ParticleSystem>();
        _audioSource = GetComponent<AudioSource>();

        _itemBaseDataSo = ItemCreateManager.Instance.CreateWeapon();
        Debug.Log("CreateWeapon 호출됨");
    }

    void Update()
    {
        
    }
}
