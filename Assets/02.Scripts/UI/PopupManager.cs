using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{

   public Stack<Popup> PopupStack = new Stack<Popup>();
   public GameObject DeliveryPopup;
   public GameObject SettingsPopup;
   
   public void CloseLastPopup()
   {
      if(PopupStack.Count > 0)
      PopupStack.Pop().ClosePopup();
   }

   public void CloseAllPopups()
   {
      while (PopupStack.Count > 0)
      {
         Popup popupToClose = PopupStack.Pop(); // 스택에서 제거
         if (popupToClose != null)
         {
            popupToClose.ClosePopup(); // 팝업 닫기 (비활성화 등)
            // 필요하다면 popupToClose.OnPoppedFromManager(); 등 호출
         }
      }
   }
   
}
