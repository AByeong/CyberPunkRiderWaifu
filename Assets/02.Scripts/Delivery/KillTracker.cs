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

   public void KillTrakerInit()
   {
       UIManager.Instance.StageMainUI.RefreshKillTrackingText(KillTrackString());  
   }

   private string KillTrackString()
   {
       int normal = GetKillCount(EnemyType.Normal);
       int elite = GetKillCount(EnemyType.Elite);
       int boss = GetKillCount(EnemyType.Boss);
       string Message = "";


       Message = ((MissionKillCount.Normal > 0) ? $"일반 적 : {normal}/{MissionKillCount.Normal}\n" : "")+
                 ((MissionKillCount.Elite > 0) ? $"엘리트 적 : {elite}/{MissionKillCount.Elite}\n" : "")+
                 ((MissionKillCount.Boss > 0) ? $"보스 : {boss}/{MissionKillCount.Boss}\n" : "");


       return Message;
   }
   
   public void IncreaseKillCount(EnemyType type)
   {
       switch (type)
       {
           case EnemyType.Normal:
                CurrentKillCount.Normal++;
               break;
           
           case EnemyType.Elite: 
                CurrentKillCount.Elite++;
               break;
           
           case EnemyType.Boss:
                CurrentKillCount.Boss++;
               break;
           
       }

       UIManager.Instance.StageMainUI.RefreshKillTrackingText(KillTrackString());

   }

   public int GetKillCount(EnemyType type)
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
