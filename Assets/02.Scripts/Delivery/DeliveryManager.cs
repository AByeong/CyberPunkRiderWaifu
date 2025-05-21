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

    private void Start()
    {
        StartDelivery();
    }

    public void StartDelivery()
    {
        Debug.Log("Starting delivery");
        UIManager.Instance.UIInit();//UI 초기화
        
        
        KillTracker.MissionKillCount = CurrentMissionData.DeliverystageData[CurrentSector].TargetKillCount;
        KillTracker.KillTrakerInit();//KillTracker초기화

        CompleteSector = CurrentMissionData.DeliverystageData.Count;

    }

    public void LoadNextSection()
    {
        if (CurrentSector == CompleteSector)
        {
            DeliveryComplete();
        }
        else
        {

            CurrentSector++;
            KillTracker.ResetCurrentKillCount();
            Debug.Log(CurrentSector);
            KillTracker.MissionKillCount = CurrentMissionData.DeliverystageData[CurrentSector].TargetKillCount;
        }
    }

    private void DeliveryComplete()
    {
        Debug.Log("Delivery complete");
        foreach (GameObject reward in CurrentMissionData.Reward.DeliveryRewards)
        {
            Debug.Log(reward.name);
        }
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
