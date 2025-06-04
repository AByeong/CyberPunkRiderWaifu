using JY;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : Singleton<UIManager>
{
    public bool ESCisClose;
    public bool isInDelivery;
    public bool isCursorLockNeed;
    public StageMainUI StageMainUI;

    [Header("팝업")]
    public PopupManager PopupManager;
    public PlayerInput PlayerInput;

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
        PopupManager = FindFirstObjectByType<PopupManager>();
        PlayerInput = FindFirstObjectByType<PlayerInput>();

        isInDelivery = false;
        isCursorLockNeed = false;

        GameObject skillBtn = GameObject.FindWithTag("SkillButton");
        if (skillBtn == null)
        {
            Debug.LogError($"{gameObject.name} :: SkillButton을 찾을 수 없습니다");
        }
        else
        {
            skillBtn.GetComponent<Button>().onClick.AddListener(LobbySkill);
        }

        GameObject shopBtn = GameObject.FindWithTag("ShopButton");
        if (shopBtn == null)
        {
            Debug.LogError($"{gameObject.name} :: ShopButton을 찾을 수 없습니다");
        }
        else
        {
            shopBtn.GetComponent<Button>().onClick.AddListener(LobbyShop);
        }
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



        }
    }

    private void InventoryPopup()
    {
        if (Input.GetKeyDown(KeyCode.I))
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
        if (Input.GetKeyDown(KeyCode.P))
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
