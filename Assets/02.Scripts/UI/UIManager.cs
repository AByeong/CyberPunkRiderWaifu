using JY;
using UnityEngine;
public class UIManager : Singleton<UIManager>
{
    public bool ESCisClose;
    public bool isInDelivery;
    public bool isCursorLockNeed;
    public StageMainUI StageMainUI;

    [Header("팝업")]
    public PopupManager PopupManager;
    public PlayerInput PlayerInput;

    public bool IsCutScenePlaying = false;

    private void Start()
    {
        PlayerInput = GameManager.Instance.player.GetComponent<PlayerInput>();
        GameManager.Instance.OnReturnToLobby += Initialize;
    }
    private void Update()
    {
        InventoryPopup();
        ShopPopup();
        SkillPopup();
        DeliveryPopup();
    }

    public void Initialize()
    {
        BindReferences();

        isInDelivery = false;
        isCursorLockNeed = false;
        IsCutScenePlaying = false;
    }

    public void BindReferences()
    {
        PopupManager = FindFirstObjectByType<PopupManager>();
        PlayerInput = FindFirstObjectByType<PlayerInput>();
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
        if (Input.GetKeyDown(KeyCode.K) && !IsCutScenePlaying)
        {
            PlayerInput.ReleaseControl();
            if (!PopupManager.SkillPopup.gameObject.activeInHierarchy)
            {

                PopupManager.SkillPopup.GetComponent<Popup>().OpenPopup();
            }
            else
            {
                PopupManager.CloseLastPopup();
                PlayerInput.GainControl();
            }
        }
    }



    private void DeliveryPopup()
    {   
        if (Input.GetKeyDown(KeyCode.Escape) && !IsCutScenePlaying)
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
                PlayerInput.GainControl();
            }
        }
    }

    private void InventoryPopup()
    {
        if (Input.GetKeyDown(KeyCode.I) && !IsCutScenePlaying)
        {
            PlayerInput.ReleaseControl();
            if (!PopupManager.InventoryPopup.gameObject.activeInHierarchy)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                PopupManager.InventoryPopup.GetComponent<Popup>().OpenPopup();
            }
            else
            {
                PopupManager.CloseLastPopup();
                PlayerInput.GainControl();
            }
        }
    }

    public void LobbyShop()
    {
        PlayerInput.ReleaseControl();

        if (!PopupManager.InventoryPopup.gameObject.activeInHierarchy)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            PopupManager.InventoryPopup.GetComponent<Popup>().OpenPopup();
        }

        if (!PopupManager.ShopPopup.gameObject.activeInHierarchy)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            PopupManager.ShopPopup.GetComponent<Popup>().OpenPopup();
        }

    }


    public void LobbySkill()
    {
        PlayerInput.ReleaseControl();
        if (!PopupManager.SkillPopup.gameObject.activeInHierarchy)
        {

            PopupManager.SkillPopup.GetComponent<Popup>().OpenPopup();
        }
        if (!PopupManager.InventoryPopup.gameObject.activeInHierarchy)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            PopupManager.InventoryPopup.GetComponent<Popup>().OpenPopup();
        }
    }

    private void ShopPopup()
    {
        if (Input.GetKeyDown(KeyCode.P) && !IsCutScenePlaying)
        {
            PlayerInput.ReleaseControl();
            if (!PopupManager.ShopPopup.gameObject.activeInHierarchy)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                PopupManager.ShopPopup.GetComponent<Popup>().OpenPopup();
            }
            else
            {
                PopupManager.CloseLastPopup();
                PlayerInput.GainControl();
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

    public void DeactivateMainUI()
    {
        if (StageMainUI == null)
        {
            Debug.LogError($"{gameObject.name} :: Stage Main UI가 없습니다!!");
        }
        StageMainUI.gameObject.SetActive(false);
    }

    public void ActivateMainUI()
    {
        if (StageMainUI == null)
        {
            Debug.LogError($"{gameObject.name} :: Stage Main UI가 없습니다!!");
        }
        StageMainUI.gameObject.SetActive(true);
    }
}
