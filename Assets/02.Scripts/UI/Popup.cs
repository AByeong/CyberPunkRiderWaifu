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
        // 기본 팝업 닫힘 시 실행될 공통 로직 (필요하다면)
        Debug.Log("Base Popup Closed. (Executing common close logic if any)");
    }

    virtual public void ClosePopup()
    {

        OnPopupClosed(); 

        gameObject.SetActive(false);
        Debug.Log("Base Popup GameObject set inactive."); // 기본 팝업 비활성화 로그 (선택 사항)
    }
}