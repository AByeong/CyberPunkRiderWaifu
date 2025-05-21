using UnityEngine;
using System;
using UnityEngine.UI;

public class DeliveryPopup : Popup
{
    public Button EscapeButton;

    private void Start()
    {
        EscapeButton.onClick.AddListener(() => {
            UIManager.Instance.PopupManager.ShowAnswerPopup(
                "정말로 그만두실 건가요?", "나갈래여", "여기 있을래요", () => Debug.Log("*****나갑니다.*******")
            );
        });
    }

    override public void OpenPopup()
    {
       
        UIManager.Instance.ESCisClose = true;
        base.OpenPopup(); 
    }

    // 팝업이 닫힐 때 DeliveryPopup 고유의 로직을 수행하기 위해 OnPopupClosed를 오버라이드합니다.
    override public void ClosePopup() 
    {
        
        UIManager.Instance.ESCisClose = false;
        base.ClosePopup();
    }
    
}