using System;
using JY;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public bool ESCisClose = false;
    public PopupManager PopupManager;
    public PlayerInput _playerInput;

    public void PlayerStop()
    {
       _playerInput.playerControllerInputBlocked = false; 
    }
    
    public void PlayerReplay()
    {
        _playerInput.playerControllerInputBlocked = true; 
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!ESCisClose)
            {
                PopupManager.DeliveryPopup.GetComponent<Popup>().OpenPopup();
            }
            else
            {
                PopupManager.CloseLastPopup();
            }
        }
    }
}
