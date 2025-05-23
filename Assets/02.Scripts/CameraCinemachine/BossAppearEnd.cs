using UnityEngine;

public class BossAppearEnd : MonoBehaviour
{

    
    
    public void BossAppearStartSignal()
    {
        Time.timeScale = 0;
    }
    
    public void BossAppearEndSignal()
    {
        CinemachineManager.Instance.BossAppearEndEvent();
        EnemyManager.Instance.SpawnBoss();
        Time.timeScale = 1;
    }
}
