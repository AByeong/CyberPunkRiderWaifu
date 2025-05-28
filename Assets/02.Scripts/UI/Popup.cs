using System;
using UnityEngine;

public abstract class Popup : MonoBehaviour
{
    
    virtual public void OpenPopup() // 매개변수 없음
    {
        
        UIManager.Instance.PopupManager.PopupStack.Push(this); 
        
        this.gameObject.SetActive(true);
        
        
        
        UIManager.Instance.CursorLock(false);
        
        Debug.Log($"현재 {UIManager.Instance.PopupManager.PopupStack.Count}개의 팝업이 열려있음");
        if (UIManager.Instance.PopupManager.PopupStack.Count > 0)
        {
            string popuplist = "";
            foreach (var popup in UIManager.Instance.PopupManager.PopupStack)
            {
                popuplist += popup.name + ", ";
            }
            Debug.Log(popuplist);
        }
    }

    // 팝업이 닫힐 때 호출될 추상/가상 메서드를 추가합니다.
    // 자식 클래스에서 이 메서드를 오버라이드하여 각자의 닫힘 로직을 구현합니다.
    protected virtual void OnPopupClosed() 
    {
        UIManager.Instance.CursorLock(true);
        
        GameManager.Instance.GameReplay();
        UIManager.Instance.ESCisClose = false;
    }

    virtual public void ClosePopup()
    {
        
        
        
        
        
        Debug.Log($"닫는순간 현재 {UIManager.Instance.PopupManager.PopupStack.Count}개의 팝업이 열려있음");
        
        if (UIManager.Instance.PopupManager.PopupStack.Count > 0)
        {
            string popuplist = "";
            foreach (var popup in UIManager.Instance.PopupManager.PopupStack)
            {
                popuplist += popup.name + ", ";
            }
            Debug.Log(popuplist);
        }
        
        if (UIManager.Instance.PopupManager.PopupStack.Count == 0)
        {
            OnPopupClosed();
        }
        
        gameObject.SetActive(false);
    }
}