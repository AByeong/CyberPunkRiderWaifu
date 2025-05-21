using Gamekit3D;
using UnityEngine;

public class GenericEquipment : IEquipable
{
    private readonly EquipmentData data;

    public string Name => data.equipmentName;

    public GenericEquipment(EquipmentData data)
    {
        this.data = data;
    }

    public void Equip()
    {
        PlayerController playerController = GameManager.Instance.player;
        playerController.ApplyEquipment(StatType.MaxHealth, data.maxHealth);

        Debug.Log($"장착: {data.equipmentName}");
    }

    public void Equip(GameObject player)
    {
        throw new System.NotImplementedException();
    }

    public void Unequip(GameObject player)
    {
        
        
        Debug.Log($"해제: {data.equipmentName}");
    }
}