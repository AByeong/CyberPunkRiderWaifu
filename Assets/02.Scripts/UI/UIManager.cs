using JY;
using UnityEngine;
using UnityEngine.Serialization;
public class UIManager : Singleton<UIManager>
{
    public bool ESCisClose;

    public StageMainUI StageMainUI;

    [Header("팝업")]
    public PopupManager PopupManager;
    [FormerlySerializedAs("_playerInput")] public PlayerInput PlayerInput;

    private void Update()
    {
        InventoryPopup();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerInput.GainControl();
            if (!ESCisClose)
            {
                PopupManager.DeliveryPopup.GetComponent<Popup>().OpenPopup();
                if (PopupManager.PopupStack.Count > 0)
                {

                    GameManager.Instance.GameStop();
                }
            }
            else
            {
                PopupManager.CloseLastPopup();

            }
        }
    }

    private void InventoryPopup()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            PlayerInput.ReleaseControl();
            if (!PopupManager.InventoryPopup.gameObject.activeInHierarchy)
            {

                PopupManager.InventoryPopup.GetComponent<Popup>().OpenPopup();
            }
            else
            {
                PopupManager.CloseLastPopup();
            }
        }
    }


    public void UIInit()
    {
        Debug.Log("UI Init");
        StageMainUI.StageMainInit();

        // StageMainUI.SkillIcons[0].CoolTime = SkillManager.Instance.EquipSkill1.SkillData.CoolTime;
        //StageMainUI.SkillIcons[1].CoolTime = SkillManager.Instance.EquipSkill2.SkillData.CoolTime;
        //StageMainUI.SkillIcons[2].CoolTime = SkillManager.Instance.EquipSkill3.SkillData.CoolTime;
        //StageMainUI.SkillIcons[3].CoolTime = SkillManager.Instance.EquipSkill4.SkillData.CoolTime;


    }


    public void PlayerStop()
    {
        PlayerInput.playerControllerInputBlocked = false;
    }

    public void PlayerReplay()
    {
        PlayerInput.playerControllerInputBlocked = true;
    }
}
