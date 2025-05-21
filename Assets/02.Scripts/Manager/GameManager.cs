using Gamekit3D;
using UnityEngine;
public class GameManager : Singleton<GameManager>
{
    public PlayerController player;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 커서를 화면 중앙에 고정
        Cursor.visible = false; // 커서 숨김
    }
}
