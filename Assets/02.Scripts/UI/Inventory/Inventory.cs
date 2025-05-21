using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject InventorySlotPrefab;
    public Transform SlotParent;
    public int SlotCount = 20;

    private InventorySlot[] slots;

    private void Start()
    {
        slots = new InventorySlot[SlotCount];

        for (int i = 0; i < SlotCount; i++)
        {
            GameObject obj = Instantiate(InventorySlotPrefab, SlotParent);
            slots[i] = obj.GetComponent<InventorySlot>();
        }
    }

    public bool AddItem(Item item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].Icon.enabled == false)
            {
                slots[i].AddItem(item);
                return true;
            }
        }

        Debug.Log("인벤토리가 가득 찼습니다!");
        return false;
    }
}
