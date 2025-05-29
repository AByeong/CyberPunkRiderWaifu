using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
   [SerializeField]
   private MonsterSpawner[] _normalMonsterSpawners = new MonsterSpawner[0];
   public MonsterSpawner[] NormalMonsterSpawners => _normalMonsterSpawners;

   [SerializeField]
   private MonsterSpawner[] _eliteMonsterSpawners = new MonsterSpawner[0];
   public MonsterSpawner[] EliteMonsterSpawners => _eliteMonsterSpawners;

   [SerializeField]
   private MonsterSpawner _bossMonsterSpawner;
   public MonsterSpawner BossMonsterSpawner => _bossMonsterSpawner;


   public void SetNormalSpwner(MonsterSpawner[] spawners)
   {
      _normalMonsterSpawners = spawners;
   }

   public void SetEliteSpawner(MonsterSpawner[] spawners)
   {
      _eliteMonsterSpawners = spawners;
   }

   public void SetBossSpawner(MonsterSpawner spawner)
   {
      _bossMonsterSpawner = spawner;
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
      _bossMonsterSpawner.StartSpawning();
   }


   public void DespawnALL()
   {
      foreach (MonsterSpawner spawner in _normalMonsterSpawners)
      {
         foreach (Transform child in spawner.transform.GetChild(0))
         {
            Enemy enemy = child.GetComponent<Enemy>();
            if (enemy != null && enemy.gameObject.activeInHierarchy == true)
            {
               enemy.Pool.ReturnObject(enemy.gameObject);
            }
         }
      }

      foreach (MonsterSpawner spawner in _eliteMonsterSpawners)
      {
         foreach (Transform child in spawner.transform.GetChild(0))
         {
            Enemy enemy = child.GetComponent<Enemy>();
            if (enemy != null && enemy.gameObject.activeInHierarchy == true)
            {
               enemy.Pool.ReturnObject(enemy.gameObject);
            }
         }   
      }
      
   }
   
}
