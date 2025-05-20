using System;
using UnityEngine;
using UnityEngine.UI;

public class Icon : MonoBehaviour
{
    [SerializeField]
    private Image[] ColorChangeable;
    [SerializeField]
    private Image IconImage;
    [SerializeField]
    private Image Loading;

    [Header("사용자 지정")]
    public float CoolTime;
    public Sprite IconImageSprite;
    public Color FrameColor;
    
    private bool _isLoading = false;
    
    public Action CoolTimeStartAction;
    public Action CoolTimeEndAction;

    private void Awake()
    {
        foreach (var frame in ColorChangeable)
        {
            frame.color = FrameColor;
            
        }
        IconImage.sprite = IconImageSprite;
        Loading.fillAmount = 0;
    }
    public void StartCooltime()
    {
        Loading.fillAmount = 1;
        CoolTimeStartAction?.Invoke();
        _isLoading = true;
    }

    private void Update()
    {
        if(!_isLoading) return;
        else
        {
            Loading.fillAmount -= Time.deltaTime / CoolTime;

            if (Loading.fillAmount <= 0)
            {
                _isLoading = false;
                CoolTimeEndAction?.Invoke();
            }
        }
    }

}
