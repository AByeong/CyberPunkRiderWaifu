using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class ForTestDummy : MonoBehaviour
{
    public EnemyManager EnemyManager;

    private bool _isActive = true;

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

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            DeliveryManager.Instance.StageManager._isClear = true;
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            if (_isActive)
            {
                UIManager.Instance.StageMainUI.gameObject.SetActive(false);
                _isActive = false;
            }
            else
            {
                UIManager.Instance.StageMainUI.gameObject.SetActive(true);
                _isActive = true;
            }
        }
        
        
        
    }
}
