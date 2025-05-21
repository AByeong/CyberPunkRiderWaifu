using UnityEngine;

public interface IEquipable
{
    string Name { get; }
    void Equip(GameObject player);
    void Unequip(GameObject player);
}
