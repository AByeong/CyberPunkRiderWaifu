using UnityEngine;
using UnityEngine.Playables;

public class ForTestDummy : MonoBehaviour
{
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
            DeliveryManager.Instance.StageManager.StageInitialize();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            DeliveryManager.Instance.StageManager._isClear = true;
        }
        
        
        
    }
}
