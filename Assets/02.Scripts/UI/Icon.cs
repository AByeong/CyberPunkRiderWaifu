using System;
using UnityEngine;
using UnityEngine.UI;
public class Icon : MonoBehaviour
{

    public enum IconType
    {
        Cooltime,
        Stack
    }
    
    [SerializeField]
    private Image[] ColorChangeable;

    public Image IconImage;
    public Image Loading;

    [Header("사용자 지정")]
    public float RestrictCondition; //쿨타임 계에서는 쿨타임을 기입, 스택 계에서는 잡아야하는 몬스터의 수를 기입
    public Sprite IconImageSprite;

    private bool _isLoading;
    public Action CoolTimeEndAction;

    public Action CoolTimeStartAction;
public IconType CoolOrStack;
    private void Awake()
    {
        
        IconImage.sprite = IconImageSprite;
        switch (CoolOrStack)
        {
            case IconType.Cooltime:
                Loading.fillAmount = 0;
                break;
            
            case IconType.Stack:
                Loading.fillAmount = 1;
                break;
        }

    }

    public void StackChange(int killcount)
    {
        Loading.fillAmount = 1 - killcount/RestrictCondition;
    }

    private void Update()
    {

        
            if (!_isLoading) return;
            Loading.fillAmount -= Time.deltaTime / RestrictCondition;

            if (Loading.fillAmount <= 0)
            {
                _isLoading = false;
                CoolTimeEndAction?.Invoke();
            }
        
        
    }

    public void StartCooltime()
    {
        Debug.Log("StartCooltime");
        Loading.fillAmount = 1;
        CoolTimeStartAction?.Invoke();
        _isLoading = true;
    }
}
