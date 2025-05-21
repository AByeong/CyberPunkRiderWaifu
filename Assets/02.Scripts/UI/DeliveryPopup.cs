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
        // DeliveryPopup이 열릴 때 고유하게 수행할 로직
        if (UIManager.Instance != null)
        {
            UIManager.Instance.PlayerStop(); // 팝업 열릴 때 플레이어 멈추기
        }
        UIManager.Instance.ESCisClose = true;
        base.OpenPopup(); 
    }

    // 팝업이 닫힐 때 DeliveryPopup 고유의 로직을 수행하기 위해 OnPopupClosed를 오버라이드합니다.
    protected override void OnPopupClosed() 
    {
        
        UIManager.Instance.PlayerReplay(); // 닫힐 때 플레이어 다시 움직이게
        UIManager.Instance.ESCisClose = false;
        base.OnPopupClosed();
    }
    
}