using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class DeliveryPopup : Popup
{
    public Button EscapeButton;
    public Image[] StageImages;
    [SerializeField] private Color _activeColor;

    public TextMeshProUGUI GetMoney;
    
    public QuestBar NormalQuestBar;
    public QuestBar EliteQuestBar;
    public QuestBar BossQuestBar;
    public QuestBar ElevatorBar;
    
    private void Start()
    {
        EscapeButton.onClick.AddListener(() => {
            UIManager.Instance.PopupManager.ShowAnswerPopup(
                "정말로 그만두실 건가요?", "나갈래여", "여기 있을래요", () =>
                {
                    Debug.Log("*****나갑니다.******씬 이름은 차후에 반드시 바꾸셔야합니다.*");
                    SceneMover.Instance.MovetoScene("KBH_Lobby");//차후 반드시 이름을 바꿀 것
                }
            
            );
        });
    }

    override public void OpenPopup()
    {
       
        UIManager.Instance.ESCisClose = true;
        base.OpenPopup(); 
        OpenSet();
        
        
    }

    // 팝업이 닫힐 때 DeliveryPopup 고유의 로직을 수행하기 위해 OnPopupClosed를 오버라이드합니다.
    override public void ClosePopup() 
    {
        
        UIManager.Instance.ESCisClose = false;
        base.ClosePopup();
    }


    private void OpenSet()
    {
        SetBars();
        StageActivate(DeliveryManager.Instance.CurrentSector);
        Money(1000);
    }

    private void SetBars()
    {
        
        NormalQuestBar.gameObject.SetActive(false);
        EliteQuestBar.gameObject.SetActive(false);
        BossQuestBar.gameObject.SetActive(false);
        ElevatorBar.gameObject.SetActive(false);
        
        if (DeliveryManager.Instance.KillTracker.MissionKillCount.Normal > 0 )
        {
            NormalQuestBar.gameObject.SetActive(true);
            NormalQuestBar.Set(DeliveryManager.Instance.KillTracker.CurrentKillCount.Normal, DeliveryManager.Instance.KillTracker.MissionKillCount.Normal,"일반 몬스터 처치", "여기 들어오시면 안 돼요!");
        }
        
        if (DeliveryManager.Instance.KillTracker.MissionKillCount.Elite > 0)
        {
            EliteQuestBar.gameObject.SetActive(true);
            EliteQuestBar.Set(DeliveryManager.Instance.KillTracker.CurrentKillCount.Elite, DeliveryManager.Instance.KillTracker.MissionKillCount.Elite, "엘리트 몬스터 처치", "긴급상황! 긴급상황!");
        }
        
            
        
        if (DeliveryManager.Instance.KillTracker.MissionKillCount.Boss > 0 )
        {
            BossQuestBar.gameObject.SetActive(true);
            BossQuestBar.Set(DeliveryManager.Instance.KillTracker.CurrentKillCount.Boss, DeliveryManager.Instance.KillTracker.MissionKillCount.Boss,"보스 처치", "서장님 도와줘요!");
        }

        if (DeliveryManager.Instance.KillTracker.IsMissionCompleted())
        {
            ElevatorBar.gameObject.SetActive(true);
        }
        
        

    }

    private void StageActivate(int number)
    {
        for (int i = 0; i < StageImages.Length; i++)
        {
            if (i == number)
            {
                StageImages[i].color = _activeColor;
            }
            else
            {
                StageImages[i].color = Color.white;
            }

        }
        
    }

    private void Money(int money)
    {
        GetMoney.text = $"<color=green>${money}</color>";
    }
    
    
    
}