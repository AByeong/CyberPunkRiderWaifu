using UnityEngine;
using UnityEngine.UI;

public class UI_CheckDropItem : Singleton<UI_CheckDropItem>
{
    public RectTransform Panel;
    public Button YesButton;
    public Button NoButton;

    private Vector2 _originPosition;
    private bool _isCalled = false;
    private UI_InventorySlot _targetSlot;

    public void TryDropItem(UI_InventorySlot slot)
    {
        _isCalled = true;
        Panel.anchoredPosition = Vector2.zero;
        _targetSlot = slot;
    }
    public void Yes()
    {
        if (_targetSlot != null && _targetSlot.HasItem)
        {
            InventoryManager.Instance.Remove(_targetSlot.Item);
            _targetSlot = null;
        }
        Panel.anchoredPosition = _originPosition;
        
    }

    public void No()
    {
        _isCalled = false;
        Panel.anchoredPosition = _originPosition;
    }
    void Start()
    {
        _originPosition = Panel.anchoredPosition;
    }

    void Update()
    {
        
    }
}
