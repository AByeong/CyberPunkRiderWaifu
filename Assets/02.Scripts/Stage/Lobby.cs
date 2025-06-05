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

        if (DeliveryManager.Instance != null)
        {
            Destroy(DeliveryManager.Instance.gameObject);
        }

        if (CinemachineManager.Instance != null)
        {
            Destroy(CinemachineManager.Instance.gameObject);
        }

        UIManager.Instance.CursorLock(false);
    }

    private void Update()
    {
        if (!_isStartDone)
        {
            _isStartDone = true;
            if (GameManager.Instance.OnReturnToLobby != null)
            {
                GameManager.Instance.OnReturnToLobby();
            }
        }
    }

    public void StartGame() => SceneMover.Instance.ShowDungeonPopup("KBJ_Procedure");
    public void Shop() => UIManager.Instance.LobbyShop();
    public void Skill() => UIManager.Instance.LobbySkill();
}
