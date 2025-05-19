using System;
using UnityEngine;

public abstract class Popup : MonoBehaviour
{
   private Action _closeAction;
   public virtual void OpenPopup(Action onClose)
   {
      _closeAction = onClose;
      UIManager.Instance.PopupManager.PopupStack.Push(this);
      this.gameObject.SetActive(true);
   }

   public virtual void ClosePopup()
   {
      _closeAction?.Invoke();
      gameObject.SetActive(false);
   }
}
