using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerSO PlayerData;
    public PlayerStats Stats;
    // public Inventory Inventory;
    // public Equipment Equipment;
    
    public void Start()
    {
        Stats = new PlayerStats(PlayerData);
    }

}
