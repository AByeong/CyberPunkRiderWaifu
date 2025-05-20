using UnityEngine;

public class SettingPopup : Popup
{
    override public void OpenPopup()
    {
        
        base.OpenPopup(); 
    }

    // 팝업이 닫힐 때 DeliveryPopup 고유의 로직을 수행하기 위해 OnPopupClosed를 오버라이드합니다.
    protected override void OnPopupClosed() 
    {
        
        base.OnPopupClosed(); 
    }
}
