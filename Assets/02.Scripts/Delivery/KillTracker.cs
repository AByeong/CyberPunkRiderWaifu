using UnityEngine;

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
