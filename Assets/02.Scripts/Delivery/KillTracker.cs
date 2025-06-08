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
       UIManager.Instance.StageMainUI.RefreshKillTracking();
       
       
       
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
       UIManager.Instance.StageMainUI.RefreshKillTracking();
       UIManager.Instance.StageMainUI.RefreshProgressbar();
       
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
