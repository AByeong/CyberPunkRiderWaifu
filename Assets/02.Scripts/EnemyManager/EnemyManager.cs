using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
   [SerializeField]
   private Queue<MonsterSpawner> _normalMonsterSpawners;
   [SerializeField]
   private Queue<MonsterSpawner> _eliteMonsterSpawners;
   [SerializeField]
   private MonsterSpawner _bossMonsterSpawners;

   public float Timer = 1f;

   protected override void Awake()
   {
      base.Awake();
      //EnemyManagerInit();
    }

   public void EnemyManagerInit()
   {
      _normalMonsterSpawners = new Queue<MonsterSpawner>();
      _eliteMonsterSpawners = new Queue<MonsterSpawner>();
   }

   public void AddNormalSpwner(MonsterSpawner spawner)
   {
      _normalMonsterSpawners.Enqueue(spawner);
   }

   public void AddEliteSpawner(MonsterSpawner spawner)
   {
      _eliteMonsterSpawners.Enqueue(spawner);
   }

   public void AddBossSpawner(MonsterSpawner spawner)
   {
      _bossMonsterSpawners = spawner;
   }

   public void RemoveNormalSpwner()
   {
      _normalMonsterSpawners.Dequeue();
   }

   public void RemoveEliteSpawner()
   {
      _eliteMonsterSpawners.Dequeue();
   }

   public void RemoveBossSpawner()
   {
      _bossMonsterSpawners = null;
   }
   

   public void InitSpawn()
   {
      foreach (MonsterSpawner spawner in _normalMonsterSpawners)
      {
         if (spawner.InitSpawn)
         {
            spawner.StartSpawning();
         }
      }
      foreach (MonsterSpawner spawner in _eliteMonsterSpawners)
      {
         if (spawner.InitSpawn)
         {
            spawner.StartSpawning();
         }
      }
   }

   

   // public void Spawn(int spawnerIndex)
   // {
   //    foreach (MonsterSpawner spawner in _normalMonsterSpawners)  // Queue로 자료형 변경으로 인한 코드 수정
   //    {
   //       spawner.StartSpawning();         
   //    }
   //    // _normalMonsterSpawners[spawnerIndex].StartSpawning();
   // }

   // public void SpawnElite(int spawnerIndex)
   // {
   //    _eliteMonsterSpawners[spawnerIndex].StartSpawning();
   // }

   public void SpawnBoss()
   {
      _bossMonsterSpawners.StartSpawning();
   }
   
}
