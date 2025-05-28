using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
   [SerializeField]
   private List<MonsterSpawner> _normalMonsterSpawners;
   public List<MonsterSpawner> NormalMonsterSpawners => _normalMonsterSpawners;

   [SerializeField]
   private List<MonsterSpawner> _eliteMonsterSpawners;
   public List<MonsterSpawner> EliteMonsterSpawners => _eliteMonsterSpawners;

   [SerializeField]
   private MonsterSpawner _bossMonsterSpawners;
   public MonsterSpawner BossMonsterSpawners => _bossMonsterSpawners;

   public float Timer = 1f;

   protected override void Awake()
   {
      base.Awake();
      EnemyManagerInit();
    }

   public void EnemyManagerInit()
   {
      _normalMonsterSpawners = new List<MonsterSpawner>();
      _eliteMonsterSpawners = new List<MonsterSpawner>();
   }

   public void AddNormalSpwner(MonsterSpawner spawner)
   {
      _normalMonsterSpawners.Add(spawner);
   }

   public void AddEliteSpawner(MonsterSpawner spawner)
   {
      _eliteMonsterSpawners.Add(spawner);
   }

   public void AddBossSpawner(MonsterSpawner spawner)
   {
      _bossMonsterSpawners = spawner;
   }

   public void RemoveNormalSpwner()
   {
      _normalMonsterSpawners.Clear();
   }

   public void RemoveEliteSpawner()
   {
      _eliteMonsterSpawners.Clear();
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
