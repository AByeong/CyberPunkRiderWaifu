using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyManager : Singleton<EnemyManager>
{
   [SerializeField]
   private MonsterSpawner[] _normalMonsterSpawners;
   [SerializeField]
   private MonsterSpawner[] _eliteMonsterSpawners;
   [SerializeField]
   private MonsterSpawner _bossMonsterSpawners;
   
   
   [SerializeField]
   private KillTracker _killTracker;
   
   void Update()
   {
      if (Input.GetKey(KeyCode.Space))
      {
         InitSpawn();
      }
   }

   private void InitSpawn()
   {
      foreach (MonsterSpawner spawner in _normalMonsterSpawners)
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
