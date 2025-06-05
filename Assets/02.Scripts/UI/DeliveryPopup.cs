using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class DeliveryPopup : Popup
{
    public Button EscapeButton;
    public TextMeshProUGUI MoneyText;

    public Image[] StageIcons;
    public Color ActiveColor;
    
    public QuestBar NormalQuestBar;
    public QuestBar EliteQuestBar;
    public QuestBar BossQuestBar;
    
    private void Start()
    {
        EscapeButton.onClick.AddListener(() => {
            UIManager.Instance.PopupManager.ShowAnswerPopup(
                "정말로 그만두실 건가요?", "나갈래여", "여기 있을래요", () =>
                {
                    Debug.Log("*****나갑니다.******씬 이름은 차후에 반드시 바꾸셔야합니다.*");
                    SceneMover.Instance.MovetoScene("KBJ_Lobby");//차후 반드시 이름을 바꿀 것
                    Time.timeScale = 1;
                    SoundManager.Instance.PlayBGM(SoundType.BGM_OfficeStage);
                    
                }
            
            );
        });
    }

    override public void OpenPopup()
    {
       
        UIManager.Instance.ESCisClose = true;
        
        MoneyText.text =  "<color=green>$</color>"+CurrencyManager.Instance.Gold.ToString();

        for (int i = 0; i < StageIcons.Length; i++)
        {
            if (i == DeliveryManager.Instance.CurrentSector)
            {
                StageIcons[i].color = ActiveColor;
            }
            else
            {
                StageIcons[i].color = Color.white;
            }
            
            
            if (DeliveryManager.Instance.KillTracker.MissionKillCount.Normal > 0 )
            {
                NormalQuestBar.gameObject.SetActive(true);
            }
            else
            {
                NormalQuestBar.gameObject.SetActive(false);
            }
        
            if (NormalQuestBar.gameObject.activeInHierarchy)
            {
                NormalQuestBar.Set(DeliveryManager.Instance.KillTracker.CurrentKillCount.Normal, DeliveryManager.Instance.KillTracker.MissionKillCount.Normal,"일반 몬스터 처치", "여기 들어오시면 안 돼요!");
            }
            else
            {
                NormalQuestBar.gameObject.SetActive(false);
            }
        
            if (DeliveryManager.Instance.KillTracker.MissionKillCount.Elite > 0)
            {
                EliteQuestBar.gameObject.SetActive(true);
            }
            else
            {
                EliteQuestBar.gameObject.SetActive(false);
            }
            
            
            
            if (EliteQuestBar.gameObject.activeInHierarchy)
            {
                EliteQuestBar.Set(DeliveryManager.Instance.KillTracker.CurrentKillCount.Elite, DeliveryManager.Instance.KillTracker.MissionKillCount.Elite, "엘리트 몬스터 처치", "긴급상황! 긴급상황!");
            }
        
            
        
            if (DeliveryManager.Instance.KillTracker.MissionKillCount.Boss > 0 )
            {
                BossQuestBar.gameObject.SetActive(true);
            }
            else
            {
                BossQuestBar.gameObject.SetActive(false);
            }
            if (BossQuestBar.gameObject.activeInHierarchy)
            {
                BossQuestBar.Set(DeliveryManager.Instance.KillTracker.CurrentKillCount.Boss, DeliveryManager.Instance.KillTracker.MissionKillCount.Boss,"보스 처치", "서장님 도와줘요!");
            }

            
            
            
            
            
            
            
            
        }
        
        base.OpenPopup(); 
    }

    // 팝업이 닫힐 때 DeliveryPopup 고유의 로직을 수행하기 위해 OnPopupClosed를 오버라이드합니다.
    override public void ClosePopup() 
    {
        
        UIManager.Instance.ESCisClose = false;
        base.ClosePopup();
    }
    
}