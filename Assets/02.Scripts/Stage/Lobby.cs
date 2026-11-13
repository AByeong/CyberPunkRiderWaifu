using UnityEngine;

public class Lobby : MonoBehaviour
{
    private bool _isStartDone = false;

    private void Start()
    {
        // Instance는 없으면 새로 만들기 때문에(빈 DeliveryManager가 생성되며 에러 발생) 기존 것만 찾아서 정리한다
        if (EnemyManager.ExistingInstance != null)
        {
            EnemyManager.ExistingInstance.DestroyEnemyManager();
        }

        if (DeliveryManager.ExistingInstance != null)
        {
            Destroy(DeliveryManager.ExistingInstance.gameObject);
        }

        if (CinemachineManager.ExistingInstance != null)
        {
            Destroy(CinemachineManager.ExistingInstance.gameObject);
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
