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
