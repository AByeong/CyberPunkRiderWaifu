using JY;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : Singleton<UIManager>
{
    public bool ESCisClose;

   public StageMainUI StageMainUI;

    [Header("팝업")]
    public PopupManager PopupManager;
    public PlayerInput _playerInput;

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


    public void UIInit()
    {
        StageMainUI.SkillIcons[0].CoolTime = SkillManager.Instance.EquipSkill1.SkillData.CoolTime;
        StageMainUI.SkillIcons[1].CoolTime = SkillManager.Instance.EquipSkill2.SkillData.CoolTime;
        StageMainUI.SkillIcons[2].CoolTime = SkillManager.Instance.EquipSkill3.SkillData.CoolTime;
        StageMainUI.SkillIcons[3].CoolTime = SkillManager.Instance.EquipSkill4.SkillData.CoolTime;
            
    }

    public void PlayerStop()
    {
        _playerInput.playerControllerInputBlocked = false;
    }

    public void PlayerReplay()
    {
        _playerInput.playerControllerInputBlocked = true;
    }

    
}
