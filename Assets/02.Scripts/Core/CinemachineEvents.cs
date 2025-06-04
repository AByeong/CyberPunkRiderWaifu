using System;
using UnityEngine;

public class CinemachineEvents : MonoBehaviour
{
    public SoundManager SoundManager;
public LoadingScene LoadingScene;
    private void Awake()
    {
        SoundManager = FindObjectOfType<SoundManager>();
    }

    private void Start()
    {
        if (SoundManager == null)
        {
            SoundManager = FindObjectOfType<SoundManager>();
        }
    }

    public void Boss2Phase()
    {
        
        
        SoundManager.Instance.PlayBGM(SoundType.BGM_Boss2Stage);
    }

    public void Skip()
    {
        LoadingScene.Skip();
    }

    public void StopBGM()
    {
        
        SoundManager.Instance.BGMSource.Stop();
    }
    
}
