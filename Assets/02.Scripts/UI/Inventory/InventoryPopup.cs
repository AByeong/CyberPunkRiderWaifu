using UnityEngine;
using UnityEngine.InputSystem;
using JY;
public class InventoryPopup : Popup
{
    override public void OpenPopup()
    {
        base.OpenPopup();
    }
    protected override void OnPopupClosed() 
    {
        base.OpenPopup();
    }
}
