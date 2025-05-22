using UnityEngine;
using UnityEngine.InputSystem;
using JY;
public class InventoryPopup : Popup
{
    override public void OpenPopup()
    {
        UIManager.Instance.PlayerInput.ReleaseControl();
        UIManager.Instance.ESCisClose = true;
        base.OpenPopup();
    }
    override public void ClosePopup() 
    {
        UIManager.Instance.PlayerInput.GainControl();
        
        UIManager.Instance.ESCisClose = false;
        base.ClosePopup();
    }
}
