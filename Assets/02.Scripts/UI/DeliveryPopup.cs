using UnityEngine;
using System;
using UnityEngine.UI;

public class DeliveryPopup : Popup
{
    public AnswerPopup EscapeButton;

    private void Start()
    {
        EscapeButton.SetupPopup("정말로 나갈 거에요?", "나갈래요", "남아 있을래요.", () => Debug.Log("나간다잉"));
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