using UnityEngine;
using System; // Action은 더 이상 직접적으로 필요 없을 수 있습니다.

public class DeliveryPopup : Popup
{
    
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

        if (UIManager.Instance != null)
        {
            UIManager.Instance.PlayerReplay(); // 닫힐 때 플레이어 다시 움직이게
        }
        
        UIManager.Instance.ESCisClose = false;
         base.OnPopupClosed(); 
    }
    
}