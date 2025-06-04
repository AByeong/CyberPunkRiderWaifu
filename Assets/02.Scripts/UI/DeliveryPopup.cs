using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class DeliveryPopup : Popup
{
    public Button EscapeButton;
    public TextMeshProUGUI MoneyText;
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
        
        base.OpenPopup(); 
    }

    // 팝업이 닫힐 때 DeliveryPopup 고유의 로직을 수행하기 위해 OnPopupClosed를 오버라이드합니다.
    override public void ClosePopup() 
    {
        
        UIManager.Instance.ESCisClose = false;
        base.ClosePopup();
    }
    
}