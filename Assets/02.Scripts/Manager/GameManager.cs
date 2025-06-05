using System;
using JY;
using UnityEngine;
public class GameManager : Singleton<GameManager>
{
    public PlayerController player;
    public GameObject WorldCanvas;

    public Action OnReturnToLobby;
    
    private void Start()
    {
        UIManager.Instance.CursorLock(true);

    }
    public void GameStop()
    {

        Time.timeScale = 0;
    }

    public void GameReplay()
    {

        Time.timeScale = 1;
    }
}
