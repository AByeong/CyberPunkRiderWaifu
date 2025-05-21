using System;
using UnityEngine;

public abstract class Popup : MonoBehaviour
{
    
    virtual public void OpenPopup() // 매개변수 없음
    {
        
        UIManager.Instance.PopupManager.PopupStack.Push(this); 
        
        this.gameObject.SetActive(true);
        
    }

    // 팝업이 닫힐 때 호출될 추상/가상 메서드를 추가합니다.
    // 자식 클래스에서 이 메서드를 오버라이드하여 각자의 닫힘 로직을 구현합니다.
    protected virtual void OnPopupClosed() 
    {
        Cursor.lockState = CursorLockMode.Locked; // 커서를 화면 중앙에 고정
        Cursor.visible = false; // 커서 숨김
        GameManager.Instance.GameReplay();
    }

    virtual public void ClosePopup()
    {

        if (UIManager.Instance.PopupManager.PopupStack.Count == 0)
        {
            OnPopupClosed();
        } 
        
        gameObject.SetActive(false);
       
    }
}