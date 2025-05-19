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
   public KillCount KillCount;
   public KillCount MissionKillCount;

   public void IncreaseKillCount(EnemyType type)
   {
       switch (type)
       {
           case EnemyType.Normal:
                KillCount.Normal++;
               break;
           
           case EnemyType.Elite: 
                KillCount.Elite++;
               break;
           
           case EnemyType.Boss:
                KillCount.Boss++;
               break;
           
       }
       
   }

   public int GetKillCount(EnemyType type)
   {
       switch (type)
       {
           case EnemyType.Normal:
               return KillCount.Normal;
           
           case EnemyType.Elite:
               return KillCount.Elite;
           
           case EnemyType.Boss:
               return KillCount.Boss;
           
           case EnemyType.Total:
               return (KillCount.Normal + KillCount.Elite + KillCount.Boss);
       }

       return -1;
   }
   
   
}
