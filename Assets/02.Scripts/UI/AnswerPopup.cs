using System;
using TMPro;
using Unity.UI;
using UnityEngine;
using UnityEngine.UI;

public class AnswerPopup : Popup
{
    public TextMeshProUGUI AnswerText;

    public Button PositiveButton;
    public TextMeshProUGUI PositiveText;
    public Button NegativeButton;
    public TextMeshProUGUI NegativeText;

    public Action ApproveAction;
    
    public void SetupPopup(string answerMessage, string positiveButtonText, string negativeButtonText, Action onApprove)
    {
        AnswerText.text = answerMessage;
        PositiveText.text = positiveButtonText;
        NegativeText.text = negativeButtonText;
        ApproveAction = onApprove;
    }
    
    override public void OpenPopup()
    {
        NegativeButton.onClick.AddListener(() => OnPopupClosed());
        PositiveButton.onClick.AddListener(() => {
            ApproveAction?.Invoke(); // ApproveAction이 할당되어 있다면 실행
            OnPopupClosed(); // 동작 실행 후 팝업 닫기
        });

        base.OpenPopup();
    }

    protected override void OnPopupClosed()
    {
        // 리스너를 제거하여 중복 구독을 방지
        NegativeButton.onClick.RemoveAllListeners();
        PositiveButton.onClick.RemoveAllListeners();

        ApproveAction = null; 

        base.OnPopupClosed();
    }
}
