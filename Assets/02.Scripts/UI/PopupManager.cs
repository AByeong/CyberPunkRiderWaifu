using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{

   public Stack<Popup> PopupStack = new Stack<Popup>();
   public GameObject DeliveryPopup;
   public GameObject SettingsPopup;
   public GameObject InventoryPopup;
   public GameObject SkillPopup;
   public GameObject ShopPopup;
   public AnswerPopup AnswerPopup;
   
   public void CloseLastPopup()
   {
      if (PopupStack.Count > 0)
      {
         PopupStack.Pop().ClosePopup();
      }
      
   }

   /// <summary>
   /// 스택 위치와 상관없이 해당 팝업을 스택에서 제거 (나머지 순서는 유지)
   /// </summary>
   public void RemoveFromStack(Popup popup)
   {
      if (popup == null || !PopupStack.Contains(popup)) return;

      Popup[] items = PopupStack.ToArray(); // [0]이 맨 위
      PopupStack.Clear();
      for (int i = items.Length - 1; i >= 0; i--)
      {
         if (items[i] != popup && items[i] != null) PopupStack.Push(items[i]);
      }
   }

   public void ShowAnswerPopup(string answerMessage, string positiveButtonText, string negativeButtonText, Action onApprove, Action onCancel=null,int numberofbuttons = 2)
   {
      Debug.Log("팝업 열림");
      AnswerPopup.SetupPopup(answerMessage, positiveButtonText, negativeButtonText, onApprove,onCancel,numberofbuttons);
      AnswerPopup.OpenPopup();
      
   }
   
   public void CloseAllPopups()
   {
      while (PopupStack.Count > 0)
      {
         Popup popupToClose = PopupStack.Pop(); // 스택에서 제거
         if (popupToClose != null)
         {
            popupToClose.ClosePopup(); // 팝업 닫기
            
         }
      }
   }
   
   
   
}
