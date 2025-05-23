using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

public class EnemyManager : Singleton<EnemyManager>
{
   [SerializeField]
   private List<MonsterSpawner> _normalMonsterSpawners;
   [SerializeField]
   private List<MonsterSpawner> _eliteMonsterSpawners;
   [SerializeField]
   private MonsterSpawner _bossMonsterSpawners;

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

   

   public void Spawn(int spawnerIndex)
   {
      _normalMonsterSpawners[spawnerIndex].StartSpawning();
   }

   public void SpawnElite(int spawnerIndex)
   {
      _eliteMonsterSpawners[spawnerIndex].StartSpawning();
   }

   public void SpawnBoss()
   {
      _bossMonsterSpawners.StartSpawning();
   }
   
}
