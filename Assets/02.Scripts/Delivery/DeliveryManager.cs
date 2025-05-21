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
        CurrentSector++;
        
        if (CurrentSector == CompleteSector)
        {
            DeliveryComplete();
        }
        else
        {

            
            KillTracker.ResetCurrentKillCount();
            Debug.Log(CurrentSector);
            KillTracker.MissionKillCount = CurrentMissionData.DeliverystageData[CurrentSector].TargetKillCount;
        }
    }

    private void DeliveryComplete()
    {
        
        
        foreach (GameObject reward in CurrentMissionData.Reward.DeliveryRewards)
        {
            Debug.Log(reward.name);
        }

        
        
        Cursor.lockState = CursorLockMode.Confined; 
        Cursor.visible = true; 
        UIManager.Instance.ESCisClose = true;
        
        
        UIManager.Instance.PopupManager.ShowAnswerPopup("축하합니다!", "우와 감사해요", "개꿀", () =>
        {
            UIManager.Instance.ESCisClose = false;
            GameManager.Instance.GameReplay();
            Debug.Log("클리어");
            UIManager.Instance.ESCisClose = false;
            UIManager.Instance.PopupManager.AnswerPopup.ClosePopup();
        },null,1);
        
        
        GameManager.Instance.GameStop();
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
