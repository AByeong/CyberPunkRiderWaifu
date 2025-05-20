using UnityEngine;
using UnityEngine.Playables;

public class ForTestDummy : MonoBehaviour
{
    public PlayableDirector CinemachineDrector;
    public EnemyManager EnemyManager;
    
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            
            EnemyManager.InitSpawn();
         
        }
        
    }
}
