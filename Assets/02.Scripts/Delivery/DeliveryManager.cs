using System;
using UnityEngine;

public class DeliveryManager : Singleton<DeliveryManager>
{
    public DeliveryMissionDataSO CurrentMissionData;
    public KillTracker KillTracker;
    public EnemyManager EnemyManager;
    public int CurrentSector;
    public int CompleteSector;
    public StageManager StageManager;
    public Action OnCompleteSector;
    
    
    
    protected override void Awake()
    {
        base.Awake();

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
        
        UIManager.Instance.UIInit();//UI 초기화
        
        KillTracker.MissionKillCount = CurrentMissionData.DeliverystageData[CurrentSector].TargetKillCount;
        CompleteSector = CurrentMissionData.DeliverystageData.Count;
        Debug.Log($"{CompleteSector}이 현재 매니져의 값이고 \n {CurrentMissionData.DeliverystageData.Count}이게 데이터의 값입니다");
        KillTracker.KillTrakerInit();//KillTracker초기화

        Debug.Log("Starting delivery");
    }

    public void CompleteCurrentSection()
    {
        //현재는 바로 바뀌지만 나중에는 완료와 전환 사이에 넣을 수 있다.
        OnCompleteSector?.Invoke();
        UIManager.Instance.StageMainUI.KillTrackingText.color = Color.cyan;
        
        //LoadNextSection();
    }

   
    public void LoadNextSection()
    {
        
        
        
        CurrentSector++;
        
        Debug.Log($"클리어까지 {CompleteSector - CurrentSector}만큼 남았습니다");
        
        if (CurrentSector == CompleteSector)
        {
            DeliveryComplete();
        }
        
        else
        {
            
            KillTracker.ResetCurrentKillCount();
            Debug.Log(CurrentSector);
            KillTracker.MissionKillCount = CurrentMissionData.DeliverystageData[CurrentSector].TargetKillCount;
            KillTracker.KillTrakerInit();
            
            StageManager.MoveNextStage();


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
            UIManager.Instance.PopupManager.CloseAllPopups();
            UIManager.Instance.PopupManager.CloseLastPopup();
            
        },null,1);
        
        
        GameManager.Instance.GameStop();
    }
   
    
    public void ChangeSectorName(int sector)
    {
        CurrentSector = sector;
    }

    


    public void RunFromDelivery()
    {
        
    }
    
}
