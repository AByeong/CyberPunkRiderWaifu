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
    public Icon[] SkillIcons;
    public Icon[] ItemIcons;
    public Icon finisherIcon;
    
    
    [Header("팝업")]
    public PopupManager PopupManager;
    public PlayerInput _playerInput;


    private void UIInit()
    {
        
    }
    
    public void PlayerStop()
    {
       _playerInput.playerControllerInputBlocked = false; 
    }
    
    public void PlayerReplay()
    {
        _playerInput.playerControllerInputBlocked = true; 
    }

    public void SkillIconLoad(int index)
    {
        SkillIcons[index].StartCooltime();
    }

    public void ItemIconLoad(int index)
    {
        ItemIcons[index].StartCooltime();
    }

    public void FinisherIconLoad()
    {
        finisherIcon.StartCooltime();
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
