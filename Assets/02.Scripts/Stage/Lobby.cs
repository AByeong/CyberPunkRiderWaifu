using System.Collections;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    private bool _isStartDone = false;

    private void Start()
    {
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.DestroyEnemyManager();
        }
    }

    private void Update()
    {
        if (!_isStartDone)
        {
            _isStartDone = true;
            GameManager.Instance.OnReturnToLobby();
        }
    }
}
