using JY;
using UnityEngine;
using UnityEngine.Serialization;
public class UIManager : Singleton<UIManager>
{
    public bool ESCisClose;
    public bool isInDelivery;
    public bool isCursorLockNeed;
    public StageMainUI StageMainUI;

    [Header("팝업")]
    public PopupManager PopupManager;
    [FormerlySerializedAs("_playerInput")] public PlayerInput PlayerInput;

    private void Update()
    {
        InventoryPopup();
        SkillPopup();
        DeliveryPopup();
    }

    public void CursorLock(bool locking)
    {
        if (locking && isCursorLockNeed)
        {
            Cursor.lockState = CursorLockMode.Locked; // 커서를 화면 중앙에 고정
            Cursor.visible = false; // 커서 숨김
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined; //커서가 화면 밖을 나가지는 못하게 함
            Cursor.visible = true; //커서 보이게 함
        }

    }

    private void SkillPopup()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayerInput.ReleaseControl();
            if (!PopupManager.SkillPopup.gameObject.activeInHierarchy)
            {

                PopupManager.SkillPopup.GetComponent<Popup>().OpenPopup();
            }
            else
            {
                PopupManager.CloseLastPopup();
            }
        }
    }

    private void DeliveryPopup()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerInput.GainControl();
            if (!ESCisClose && isInDelivery)
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


            //     if (isInDelivery)
            //     {
            //
            //         PlayerInput.GainControl();
            //         if (!ESCisClose)
            //         {
            //             PopupManager.DeliveryPopup.GetComponent<Popup>().OpenPopup();
            //             if (PopupManager.PopupStack.Count > 0)
            //             {
            //
            //                 GameManager.Instance.GameStop();
            //             }
            //         }
            //         else
            //         {
            //             PopupManager.CloseLastPopup();
            //
            //         }
            //     }
            // }
            // else
            // {
            //     PopupManager.CloseLastPopup();
            //
        }
    }

    private void InventoryPopup()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            PlayerInput.ReleaseControl();
            if (!PopupManager.InventoryPopup.gameObject.activeInHierarchy)
            {
                Cursor.lockState = CursorLockMode.Confined; // 커서를 화면 중앙에 고정
                Cursor.visible = true; // 커서 숨김
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
