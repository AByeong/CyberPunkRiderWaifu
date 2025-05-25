using System;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    private SphereCollider _collider;
    private Item _item;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == true)
        {
            // Equipment, Chip, Etc 일 경우 (Etc는 추가될 소비아이템) -> 소비아이템 인벤에 들어오는 형식 아니면 추후 수정
            InventoryManager.Instance.AddItemToInventory(_item);
            Destroy(this.gameObject);
            //InventoryManager.Instance.
            // Gold는 추후 UI 생기면 거기에 +
        }
    }

    void Start()
    {
        _collider = GetComponent<SphereCollider>();
        ItemCreateManager.Instance.CreateWeapon();
    }

    void Update()
    {
        
    }
}
