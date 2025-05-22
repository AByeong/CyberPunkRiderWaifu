using Gamekit3D;
using UnityEngine;

public class GenericEquipment : IEquipable
{
    private readonly EquipmentData data;

    public string Name => data.EquipmentName;

    public GenericEquipment(EquipmentData data)
    {
        this.data = data;
    }

    public void Equip()
    {
        PlayerController playerController = GameManager.Instance.player;
        playerController.ApplyEquipment(StatType.MaxHealth, data.MaxHealth);

        Debug.Log($"장착: {data.EquipmentName}");
    }

    public void Equip(GameObject player)
    {
        throw new System.NotImplementedException();
    }

    public void Unequip(GameObject player)
    {
        
        
        Debug.Log($"해제: {data.EquipmentName}");
    }
}