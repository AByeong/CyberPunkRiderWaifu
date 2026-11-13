using UnityEngine;
public abstract class Popup : MonoBehaviour
{

    public virtual void OpenPopup() // 매개변수 없음
    {
        // 이미 열려 있던 팝업이 다시 열려도 스택에 중복으로 쌓이지 않게 한다
        UIManager.Instance.PopupManager.RemoveFromStack(this);
        UIManager.Instance.PopupManager.PopupStack.Push(this);
        SoundManager.Instance.Play(SoundType.UI_Open);
        this.gameObject.SetActive(true);
        UIManager.Instance.CursorLock(false);

        //  Debug.Log($"현재 {UIManager.Instance.PopupManager.PopupStack.Count}개의 팝업이 열려있음");
        if (UIManager.Instance.PopupManager.PopupStack.Count > 0)
        {
            string popuplist = "";
            foreach(Popup popup in UIManager.Instance.PopupManager.PopupStack)
            {
                popuplist += popup.name + ", ";
            }
//            Debug.Log(popuplist);
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

    public virtual void ClosePopup()
    {
        
        SoundManager.Instance.Play(SoundType.UI_Close);

        // 버튼 등에서 ClosePopup()을 직접 호출해도 스택에 남지 않도록 자기 자신을 제거
        // (CloseLastPopup/CloseAllPopups 경로에서는 이미 Pop된 상태라 아무 일도 하지 않음)
        UIManager.Instance.PopupManager.RemoveFromStack(this);

        if (UIManager.Instance.PopupManager.PopupStack.Count == 0)
        {
            OnPopupClosed();
        }
        
        gameObject.SetActive(false);
    }
}
