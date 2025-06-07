using System;
using UnityEngine;

public class CinemachineEvents : MonoBehaviour
{
public LoadingScene LoadingScene;
    private void Awake()
    {
    }

    private void Start()
    {
        
    }

    public void Boss2Phase()
    {
        
        SoundManager.Instance.PlayBGM(SoundType.BGM_Boss2Stage);
    }

    public void Skip()
    {
        LoadingScene.Skip();
    }

    
    public void AnimationStartSet()
    {
        UIManager.Instance.StageMainUI.gameObject.SetActive(false);

    }
    
    public void StopBGM()
    {
        
        SoundManager.Instance.BGMSource.Stop();
    }
    
}
