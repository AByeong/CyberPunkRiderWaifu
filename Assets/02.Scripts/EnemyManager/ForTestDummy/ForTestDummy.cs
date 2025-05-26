using UnityEngine;
using UnityEngine.Playables;

public class ForTestDummy : MonoBehaviour
{
    public PlayableDirector CinemachineDrector;
    public EnemyManager EnemyManager;
    
    void Update()
    {
        if (Input.GetKey(KeyCode.H))
        {
            
            EnemyManager.InitSpawn();
         
        }
        
        if (Input.GetKey(KeyCode.L))
        {
            
            UIManager.Instance.StageMainUI.SkillIconLoad(1);
         
        }
        
        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            StageManager.Instance.StageInitialize();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            StageManager.Instance._isClear = true;
        }
        
    }
}
