using System;
using UnityEngine;

public class DeliveryManager : Singleton<DeliveryManager>
{
    public DeliveryMissionDataSO CurrentMissionData;
    public KillTracker KillTracker;
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
        UIManager.Instance.UIInit();//UI 초기화
        
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
