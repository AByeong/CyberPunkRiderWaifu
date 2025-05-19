using System;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public DeliveryMissionDataSO CurrentMissionData;
    public EnemyManager EnemyManager;
    public int CurrentSector;
    public int CompleteSector;
    
    private void Awake()
    {
        if (CurrentMissionData == null)
        {
            Debug.LogError("CurrentMissionData is null");
        }

        if (EnemyManager == null)
        {
            EnemyManager = FindAnyObjectByType<EnemyManager>().GetComponent<EnemyManager>();
        }
    }

    public void StartDelivery()
    {
        
    }

    public void LoadNextSection()
    {
        
    }

    public void ChangeSectorName(int sector)
    {
        CurrentSector = sector;

        if (CurrentSector == CompleteSector)
        {
            ComplteDelivery();
        }
    }

    private void ComplteDelivery()
    {
        
    }


    public void RunFromDelivery()
    {
        
    }
    
}
