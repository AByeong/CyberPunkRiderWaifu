using System;
using TMPro;
using UnityEngine.UI;

public class AnswerPopup : Popup
{
    public TextMeshProUGUI AnswerText;

    public Button PositiveButton;
    public TextMeshProUGUI PositiveText;
    public Button NegativeButton;
    public TextMeshProUGUI NegativeText;

    public Action ApproveAction;
    public Action CancelAction;
    
    public void SetupPopup(string answerMessage, string positiveButtonText, string negativeButtonText, Action onApprove, Action onCancel = null, int numberofbutton = 2)
    {
        AnswerText.text = answerMessage;
        PositiveText.text = positiveButtonText;
        NegativeText.text = negativeButtonText;
        ApproveAction = onApprove;
        
        if (onCancel == null)
        {
            NegativeButton.onClick.AddListener(() => ClosePopup());
        }
        else
        {
            NegativeButton.onClick.AddListener(() => onCancel());
        }

        switch (numberofbutton)
        {
            case 1: 
                NegativeButton.gameObject.SetActive(false);
                break;
            
            case 2: 
                NegativeButton.gameObject.SetActive(true);
                break;
            
        }
        
        
    }
    
    override public void OpenPopup()
    {
        PositiveButton.onClick.AddListener(() => {
            ApproveAction?.Invoke(); // ApproveAction이 할당되어 있다면 실행

        });

        base.OpenPopup();
    }

    override public void ClosePopup()
    {
        
        
        

        base.ClosePopup();
    }
}
