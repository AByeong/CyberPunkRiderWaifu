using System;
using JY;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public bool ESCisClose = false;
    
    [Header("메인 UI")]
    public Slider HPSlider;
    public Slider ProgressSlider;
    public Image[] SkillImages;
    public Image[] ItemImages;
    
    
    [Header("팝업")]
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
