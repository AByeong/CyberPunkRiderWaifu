using System;
using UnityEngine;

[Serializable]
public struct KillCount
{
    public int Normal;
    public int Elite;
    public int Boss;
}

public enum EnemyType
{
    Normal,
    Elite,
    Boss,
    Total,
    
    
    
    
    Count
}




public class KillTracker : MonoBehaviour
{
   public KillCount CurrentKillCount;
   public KillCount MissionKillCount;
   public int TotalKillCount = 0;
   public void KillTrakerInit()
   {
       Debug.Log("KillTraker Init");
       
       UIManager.Instance.StageMainUI.RefreshKillTracking(KillTrackString());
       
       //지금 미션에서의 킬카운트 가져오기
   }

   private string KillTrackString()
   {
       int normal = GetCurrentKillCount(EnemyType.Normal);
       int elite = GetCurrentKillCount(EnemyType.Elite);
       int boss = GetCurrentKillCount(EnemyType.Boss);
       string Message = "";


       Message = ((MissionKillCount.Normal > 0) ? $"일반 적 : {normal}/{MissionKillCount.Normal}\n" : "")+
                 ((MissionKillCount.Elite > 0) ? $"엘리트 적 : {elite}/{MissionKillCount.Elite}\n" : "")+
                 ((MissionKillCount.Boss > 0) ? $"보스 : {boss}/{MissionKillCount.Boss}\n" : "");

    Debug.Log(Message);
       return Message;
   }

   public void ResetCurrentKillCount()
   {
       CurrentKillCount.Normal = 0;
       CurrentKillCount.Elite = 0;
       CurrentKillCount.Boss = 0;
   }
   public void IncreaseKillCount(EnemyType type)
   {
       switch (type)
       {
           case EnemyType.Normal:
               if(GetCurrentKillCount(EnemyType.Normal)!=GetMissionKillCount(EnemyType.Normal))
                CurrentKillCount.Normal++;
               break;
           
           case EnemyType.Elite: 
               if(GetCurrentKillCount(EnemyType.Elite) != GetMissionKillCount(EnemyType.Elite))
                CurrentKillCount.Elite++;
               break;
           
           case EnemyType.Boss:
               if(GetCurrentKillCount(EnemyType.Boss) != GetMissionKillCount(EnemyType.Boss))
                CurrentKillCount.Boss++;
               break;
           
       }

       if (IsMissionCompleted())
       {
           Debug.Log("Mission Completed");
           DeliveryManager.Instance.CompleteCurrentSection();
           
       }

       IncreaseTotalKillCount();
       UIManager.Instance.StageMainUI.RefreshKillTracking(KillTrackString());
       
   }


   public void ResetTotalKillCount()
   {
       TotalKillCount = 0;
   }
   private void IncreaseTotalKillCount()
   {
       TotalKillCount++;
   }

   public int GetCurrentKillCount(EnemyType type)
   {
       switch (type)
       {
           case EnemyType.Normal:
               return CurrentKillCount.Normal;
           
           case EnemyType.Elite:
               return CurrentKillCount.Elite;
           
           case EnemyType.Boss:
               return CurrentKillCount.Boss;
           
           case EnemyType.Total:
               return (CurrentKillCount.Normal + CurrentKillCount.Elite + CurrentKillCount.Boss);
       }

       return -1;
   }
   
   public int GetMissionKillCount(EnemyType type)
   {
       switch (type)
       {
           case EnemyType.Normal:
               return MissionKillCount.Normal;
           
           case EnemyType.Elite:
               return MissionKillCount.Elite;
           
           case EnemyType.Boss:
               return MissionKillCount.Boss;
           
           case EnemyType.Total:
               return (MissionKillCount.Normal + MissionKillCount.Elite + MissionKillCount.Boss);
       }

       return -1;
   }

   public bool IsMissionCompleted()
   {
       if (MissionKillCount.Normal == CurrentKillCount.Normal && MissionKillCount.Elite == CurrentKillCount.Elite && MissionKillCount.Boss == CurrentKillCount.Boss)
       {
           return true;
       }
       else
       {
           return false;
       }
       
       
   }
   
}
